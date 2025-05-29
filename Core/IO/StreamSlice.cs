using System;
using System.IO;

namespace UPKSuite.Core.IO
{
    /// <summary>
    /// Provides a view over a portion of a stream with a specified length.
    /// </summary>
    public class StreamSlice : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _startPosition;
        private readonly long _length;
        private long _position;

        /// <summary>
        /// Creates a new stream slice that limits reading to the specified number of bytes.
        /// </summary>
        /// <param name="baseStream">The source stream to read from</param>
        /// <param name="length">The maximum number of bytes that can be read</param>
        public StreamSlice(Stream baseStream, long length)
        {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative");
            
            _baseStream = baseStream;
            _startPosition = baseStream.Position;
            _length = length;
            _position = 0;
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => false; // This is a read-only slice
        public override long Length => _length;
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // Calculate how many bytes we can read
            int bytesToRead = (int)Math.Min(count, _length - _position);
            if (bytesToRead <= 0)
                return 0; // End of slice reached

            // Read from the base stream
            int bytesRead = _baseStream.Read(buffer, offset, bytesToRead);
            _position += bytesRead;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition;
            
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = _position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = _length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            if (newPosition < 0 || newPosition > _length)
                throw new ArgumentOutOfRangeException(nameof(offset), "Attempt to seek outside the bounds of the stream slice");

            _baseStream.Position = _startPosition + newPosition;
            _position = newPosition;
            return _position;
        }

        public override void Flush() => _baseStream.Flush();
        
        public override void SetLength(long value) => 
            throw new NotSupportedException("StreamSlice does not support changing the length");
        
        public override void Write(byte[] buffer, int offset, int count) => 
            throw new NotSupportedException("StreamSlice is read-only");
        
        protected override void Dispose(bool disposing)
        {
            // We don't dispose the base stream as it might be used elsewhere
            base.Dispose(disposing);
        }
    }
}
