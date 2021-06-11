using Pdfium2BMP;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace AccessVoilationBug
{
    public sealed class PdfRenderer : IDisposable
    {
        private readonly FileStream _fileStream;
        private readonly PdfDocument _document;

        public PdfRenderer(FileInfo input, int dpi = 300)
        {
            _fileStream = input.OpenRead();
            _document = PdfDocument.Load(_fileStream);
            DPI = dpi;
        }

        public int DPI { get; }
        public int PageCount => _document.PageCount;

        public ImageStream Render(int page)
        {
            page = page - 1;
            var imageBytes = _document.Render(page, DPI, DPI, PdfRenderFlags.CorrectFromDpi);

            using (var img = Image.LoadPixelData<Bgra32>(imageBytes.DataInBgra, imageBytes.Width, imageBytes.Height))
            {
                var memoryStream = new MemoryStream();
                img.SaveAsPng(memoryStream);
                memoryStream.Position = 0;
                return new ImageStream(memoryStream, img.Width, img.Height);
            }
        }

        public void Dispose()
        {
            _document.Dispose();
            _fileStream.Dispose();
        }
    }

    public sealed class ImageStream : Stream
    {
        private readonly Stream _stream;

        internal ImageStream(Stream stream, int width, int height)
        {
            _stream = stream;
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        public override bool CanRead
            => _stream.CanRead;

        public override bool CanSeek
            => _stream.CanSeek;

        public override bool CanWrite
            => _stream.CanWrite;

        public override long Length
            => _stream.Length;

        public override long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public override void Flush()
            => _stream.Flush();

        public override int Read(byte[] buffer, int offset, int count)
            => _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
            => _stream.Seek(offset, origin);

        public override void SetLength(long value)
            => _stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
            => _stream.Write(buffer, offset, count);
    }
}
