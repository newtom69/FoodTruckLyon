using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf
{
    class Program
    {
        static void Main(string[] args)
        {


            HtmlToPdf htmlToPdf = new HtmlToPdf();
            PdfDocument documentPdf = htmlToPdf.ConvertUrl("http://www.foodtrucklyon.fr");

            documentPdf.Save("d:/toto.pdf");
            documentPdf.Close();

        }
    }
}
