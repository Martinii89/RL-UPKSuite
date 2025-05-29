using RlUpk.Core.Classes;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;
using RlUpk.Core.Utility;

namespace DummyPackageBuilderCli
{
    public class DummyPackageBuilder(string packageName)
    {
        private readonly List<string> _names = ["Class", "Core", "None", "Package"];

        private readonly HashSet<ExportObjectReference> _exportObjectsSet = [];

        private readonly HashSet<ImportObjectReference> _importObjectsSet = [];

        private PackageCache? _packageCache;

        private NativeClassFactory? _nativeFactory;

        private bool _graphLink = true;

        public UnrealPackage BuildPackage()
        {
            var pck = new UnrealPackage
            {
                PackageName = packageName, PackageCache = _packageCache, NativeClassFactory = _nativeFactory
            };
            pck.PackageCache?.AddPackage(pck);
            pck.Header = FileSummary.CreateWithDefaults();
            var import = _importObjectsSet.ToList();
            var exports = _exportObjectsSet.ToList();
            AddNames(pck);
            AddImports(pck, import, exports);
            AddExports(pck, import, exports);
            if (_graphLink)
            {
                pck.GraphLink();
            }

            return pck;
        }

        private void AddExports(UnrealPackage pck, List<ImportObjectReference> imports, List<ExportObjectReference> exports)
        {
            foreach (ExportObjectReference exportObject in exports)
            {
                var classIndex =
                    imports.FindIndex(o => exportObject.Class == o.Name && exportObject.ClassPackage == o.Outer);
                var outerIndex = exports.FindIndex(o => exportObject.Parent == o.Name);
                var export = new ExportTableItem()
                {
                    ClassIndex = new ObjectIndex(ObjectIndex.FromImportIndex(classIndex)),
                    OuterIndex = new ObjectIndex(ObjectIndex.FromExportIndex(outerIndex)),
                    ObjectName = pck.GetOrAddName(exportObject.Name),
                    NetObjects = [],
                    PackageGuid = new FGuid(),
                    SerialSize = -1
                };
                pck.ExportTable.Add(export);
            }
        }

        private void AddImports(UnrealPackage pck, List<ImportObjectReference> imports, List<ExportObjectReference> exports)
        {
            if (exports.Any(x => x is { Class: "Package", ClassPackage: "Core" }))
            {
                imports.Add(new ImportObjectReference(null, "Core", "Core", "Package"));
                imports.Add(new ImportObjectReference("Core", "Package", "Core", "Class"));
            }

            foreach (ImportObjectReference importObject in imports)
            {
                int outerIndex = imports.FindIndex(x => x.Name == importObject.Outer);
                var import = new ImportTableItem()
                {
                    ClassPackage = pck.GetOrAddName(importObject.ClassPackage),
                    ClassName = pck.GetOrAddName(importObject.Class),
                    ObjectName = pck.GetOrAddName(importObject.Name),
                    OuterIndex = new ObjectIndex(ObjectIndex.FromImportIndex(outerIndex)),
                };
                pck.ImportTable.Add(import);
            }
        }

        private void AddNames(UnrealPackage pck)
        {
            foreach (string name in _names.Distinct())
            {
                pck.GetOrAddName(name);
            }
        }

        public void AddObject(string? group, string name, string classFullName)
        {
            SplitAndAddNames(group);
            SplitAndAddNames(name);
            SplitAndAddNames(classFullName);

            var firstDot = classFullName.IndexOf('.');
            var classPackage = classFullName[..firstDot];
            var className = classFullName[(firstDot + 1)..];

            // Add Import for the package of the required class
            _importObjectsSet.Add(new ImportObjectReference(null, classPackage, "Core", "Package"));
            // Add Import for the required class
            _importObjectsSet.Add(new ImportObjectReference(classPackage, className, "Core", "Class"));
        
        
            var parentParts = (group?.Split('.') ?? Enumerable.Empty<string>()).ToList();
            var links = new LinkedList<string>(parentParts);
            links.AddLast(name);
            var current = links.First;
            string? previousValue = null;
            while (current != null)
            {
                switch (current.Next)
                {
                    case null:
                        // Add export for object
                        _exportObjectsSet.Add(new ExportObjectReference(previousValue, current.Value, classPackage,
                            className));
                        break;
                    default:
                        // Add exports for parent packages
                        _exportObjectsSet.Add(new ExportObjectReference(previousValue, current.Value, "Core", "Package"));
                        break;
                }

                previousValue = current.Value;
                current = current.Next;
            }
        }

        private void SplitAndAddNames(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var parts = name.Split(".");
            foreach (string part in parts)
            {
                _names.Add(part);
            }
        }

        public DummyPackageBuilder WithPackageCache(PackageCache packageCache)
        {
            _packageCache = packageCache;
            return this;
        }

        public DummyPackageBuilder WithNativeFactory(NativeClassFactory? nativeFactory)
        {
            _nativeFactory = nativeFactory;
            return this;
        }

        public DummyPackageBuilder SkipGraphLinkPackage()
        {
            _graphLink = false;
            return this;
        }
    }

    record ExportObjectReference(string? Parent, string Name, string ClassPackage, string Class);

    record ImportObjectReference(string? Outer, string Name, string ClassPackage, string Class);
}