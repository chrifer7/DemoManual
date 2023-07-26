using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using DemoManual.Model;
using DemoManual.View;

namespace DemoManual
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            var data = LoadDataFromXml();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
            MainForm form = new MainForm();
            form.dataGridView1.DataSource = data;
            form.ShowDialog();
        }

        private static List<Book> LoadDataFromXml()
        {
            string xmlFilePath = "Books.xml";

            if (!File.Exists(xmlFilePath))
            {
                MessageBox.Show("XML file not found!");
                return new List<Book>();
            }

            // Deserialize the XML file to a list of Book objects
            List<Book> books;
            XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));
            using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
            {
                books = (List<Book>)serializer.Deserialize(fileStream);
            }

            return books;
        }
    }
}
