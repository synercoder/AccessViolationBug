using AccessVoilationBug;
using System.IO;
using Xunit;

namespace AccessViolationBug.Tests
{
    public class TriggerErrorOn50301
    {
        [Fact]
        public void Test()
        {

            var file = new FileInfo("simple.pdf");
            using (var renderer = new PdfRenderer(file))
            {
                using (var image = renderer.Render(1))
                {
                    // do stuff with image
                    // write to file or whatever
                }
            } // renderer.dispose triggers the bug on 5.0.301, but not on 5.0.300
        }
    }
}
