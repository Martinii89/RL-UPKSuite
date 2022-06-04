﻿using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultMaterialSerializer : BaseObjectSerializer<UMaterial>
{
    private readonly IStreamSerializerFor<FMaterialResource> _materialResourceSerializer;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMaterialSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<FMaterialResource> materialResourceSerializer)
    {
        _objectSerializer = objectSerializer;
        _materialResourceSerializer = materialResourceSerializer;
    }

    public override void DeserializeObject(UMaterial obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        if (obj.Name == "Body_Paintable_Mat")
        {
            Debugger.Break();
        }

        obj.FMaterialResources[0] = _materialResourceSerializer.Deserialize(objectStream);
        var leftOver = obj.ExportTableItem.SerialOffset + obj.ExportTableItem.SerialSize - objectStream.Position;
        obj.FMaterialResources[0].UnknownBytes = objectStream.ReadBytes((int) leftOver);
    }

    public override void SerializeObject(UMaterial obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class FmaterialResourceSerializer : IStreamSerializerFor<FMaterialResource>
{
    private readonly IStreamSerializerFor<FGuid> _fguidSerializer;
    private readonly IStreamSerializerFor<FName> _fnameSerializer;

    private readonly IStreamSerializerFor<FString> _fstringSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    public FmaterialResourceSerializer(IStreamSerializerFor<FString> fstringSerializer, IStreamSerializerFor<FName> fnameSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FGuid> fguidSerializer)
    {
        _fstringSerializer = fstringSerializer;
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _fguidSerializer = fguidSerializer;
    }

    public FMaterialResource Deserialize(Stream stream)
    {
        var res = new FMaterialResource();

        try
        {
            res.CompileErrors = _fstringSerializer.ReadTArrayToList(stream).Select(x => x.InnerString).ToList();
            res.TextureDependencyLengthMap = stream.ReadDictionary(ReadObjectIndex, ReadInt);
            res.MaxTextureDependencyLength = stream.ReadInt32();
            res.ID = _fguidSerializer.Deserialize(stream);
            res.NumUserTexCoords = stream.ReadUInt32();
            res.UniformExpressionTextures = _objectIndexSerializer.ReadTArrayToList(stream);
            res.bUsesSceneColorTemp = stream.ReadInt32() == 1;
            res.bUsesSceneDepthTemp = stream.ReadInt32() == 1;
            res.bUsesDynamicParameterTemp = stream.ReadInt32() == 1;
            res.bUsesLightmapUVsTemp = stream.ReadInt32() == 1;
            res.bUsesMaterialVertexPositionOffsetTemp = stream.ReadInt32() == 1;
            res.UsingTransforms = stream.ReadInt32() == 1;
            res.FTextureLookupInfos = stream.ReadTarray(stream1 => new FTextureLookupInfo
            {
                TexCoordIndex = stream.ReadInt32(),
                TextureIndex = stream.ReadInt32(),
                UScale = stream.ReadSingle(),
                VScale = stream.ReadSingle()
            });

            res.UnknownBytes = stream.ReadBytes(16);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return res;
    }

    public void Serialize(Stream stream, FMaterialResource value)
    {
        throw new NotImplementedException();
    }

    internal ObjectIndex ReadObjectIndex(Stream stream)
    {
        return _objectIndexSerializer.Deserialize(stream);
    }

    internal FName ReadFName(Stream stream)
    {
        return _fnameSerializer.Deserialize(stream);
    }

    internal int ReadInt(Stream stream)
    {
        return stream.ReadInt32();
    }
}