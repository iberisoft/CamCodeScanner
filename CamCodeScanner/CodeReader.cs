using System.Drawing;
using ZXing;

namespace CamCodeScanner
{
    class CodeReader
    {
        readonly BarcodeReader m_Reader = new BarcodeReader();

        public CodeReader()
        {
            m_Reader.Options.PossibleFormats = new[] { BarcodeFormat.QR_CODE };
        }

        public string Decode(Bitmap bitmap) => m_Reader.Decode(bitmap)?.Text;
    }
}
