// Developer Express Code Central Example:
// How to implement CRUD operations using XtraGrid and WCF Data Services
// 
// This example is similar to the http://www.devexpress.com/scid=E3866 example and
// demonstrates how to implement CRUD operations using XtraGrid and WCF Data
// Services.
// 
// This example works with the standard SQL Northwind database.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4365

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Example {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
