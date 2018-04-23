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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Data.WcfLinq;
using System.Data.Services.Client;
using Example.ServiceReference1;

namespace Example {
    public partial class MainForm : Form {
        private NorthwindEntities context;    
        WcfServerModeSource wSMS;
        private Customers newCustomer;   
        private Customers customerToEdit;
        int OldRowCount = 0;
        string newCustomerKey = String.Empty;
        int SelectedRow = -1;
        public MainForm() {
            InitializeComponent();
            context = new NorthwindEntities(new Uri(@"http://localhost:56848/NorthwindWcfDataService.svc/"));                      
            wSMS = new WcfServerModeSource();
            wSMS.KeyExpression += "CustomerID";
            wSMS.Query = context.Customers;
            gridControl1.DataSource = wSMS;            
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            newCustomer = CreateNewCustomer();
            OldRowCount = gridView1.DataRowCount;
            newCustomerKey = newCustomer.CustomerID;
            EditCustomer(newCustomer, "Add new customer", CloseAddNewCustomerHandler);
        }
        private void CloseAddNewCustomerHandler(object sender, EventArgs e)
        {
            if (((EditForm)sender).DialogResult == DialogResult.OK)
            {
                context.AddToCustomers(newCustomer);
                SaveChandes();
            }
            newCustomer = null;
        }
        private Customers CreateNewCustomer()
        {
            var newCustomer = new Customers();
            newCustomer.CustomerID = GenerateCustomerID();
            return newCustomer;
        }

        private string GenerateCustomerID()
        {
            const int IDLength = 5;
            var result = String.Empty;
            var rnd = new Random();
            for (var i = 0; i < IDLength; i++)
            {
                result += Convert.ToChar(rnd.Next(65, 90));
            }
            return result;
        }

        private void EditCustomer(Customers customer, string windowTitle, FormClosingEventHandler closedDelegate)
        {
            var frm = new EditForm(customer);
            frm.Text = windowTitle;
            frm.FormClosing += closedDelegate;
            frm.ShowDialog();
        }

        private void SaveChandes()
        {
            IAsyncResult asyncResult = null;
            try
            {
                asyncResult = context.BeginSaveChanges(SaveCallback, null);
            }
            catch (Exception ex)
            {
                context.CancelRequest(asyncResult);
                HandleException(ex);
            }
        }

        private void SaveCallback(IAsyncResult asyncResult)
        {
            DataServiceResponse response = null;
            try
            {
                response = context.EndSaveChanges(asyncResult);
            }
            catch (Exception ex)
            {
                gridControl1.BeginInvoke((MethodInvoker)delegate { context.CancelRequest(asyncResult); });
                HandleException(ex);
                gridControl1.BeginInvoke((MethodInvoker)delegate { DetachFailedEntities(); });
            }
            gridControl1.BeginInvoke((MethodInvoker)delegate { wSMS.Reload();  });
            gridControl1.BeginInvoke((MethodInvoker)delegate {
                if (SelectedRow > 0)
                    gridView1.FocusedRowHandle = SelectedRow;
                if (!String.IsNullOrEmpty(newCustomerKey) && gridView1.DataRowCount > OldRowCount)
                {
                    for (int i = 0; i < gridView1.DataRowCount; i++)
                    {
                        if (newCustomerKey == gridView1.GetRowCellValue(i, gridView1.Columns["CustomerID"]).ToString())
                        {
                            gridView1.FocusedRowHandle = i;
                            OldRowCount = gridView1.DataRowCount;
                            newCustomerKey = String.Empty;
                            break;
                        }
                    }
                }
            });
        }

        private void DetachFailedEntities()
        {
            foreach (EntityDescriptor entityDescriptor in context.Entities)
            {
                if (entityDescriptor.State != EntityStates.Unchanged)
                {
                    context.Detach(entityDescriptor.Entity);
                }
            }
        }

        private void HandleException(Exception ex)
        {
            gridControl1.BeginInvoke((MethodInvoker)delegate { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK); });
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SelectedRow = gridView1.FocusedRowHandle;
            EditSelectedCustomer(gridView1.FocusedRowHandle);
        }

        private void EditSelectedCustomer(int rowHandle)
        {
            if (rowHandle < 0)
            {
                return;
            }
            FindCustomerByIDAndProcess(GetCustomerIDByRowHandle(rowHandle), customer =>
            {
                customerToEdit = customer;
                EditCustomer(customerToEdit, "Edit customer", CloseEditCustomerHandler);
            });
        }

        private string GetCustomerIDByRowHandle(int rowHandle)
        {
            return (string)gridView1.GetRowCellValue(rowHandle, "CustomerID");
        }

        private void CloseEditCustomerHandler(object sender, EventArgs e)
        {
            if (((EditForm)sender).DialogResult == DialogResult.OK)
            {
                context.UpdateObject(customerToEdit);
                SaveChandes();
                
            }
            customerToEdit = null;
        }

        private void FindCustomerByIDAndProcess(string customerID, Action<Customers> action)
        {
            var query = (DataServiceQuery<Customers>)context.Customers.Where<Customers>(customer => customer.CustomerID == customerID);
            try
            {
                query.BeginExecute(FindCustomerByIDCallback, new QueryAction(query, action));
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void FindCustomerByIDCallback(IAsyncResult ar)
        {
            var state = (QueryAction)ar.AsyncState;
            var customers = state.Query.EndExecute(ar);
            foreach (Customers customer in customers)
            {
                try
                {
                    gridControl1.BeginInvoke((MethodInvoker)delegate { state.Action(customer); });
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }  
   
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            SelectedRow = gridView1.FocusedRowHandle;
            DeleteSelectedCustomer(gridView1.FocusedRowHandle);

        }

        private void DeleteSelectedCustomer(int rowHandle)
        {
            if (rowHandle < 0)
            {
                return;
            }
            if (MessageBox.Show("Do you really want to delete the selected customer?", "Delete Customer", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            FindCustomerByIDAndProcess(GetCustomerIDByRowHandle(rowHandle), customer =>
            {
                context.DeleteObject(customer);
                SaveChandes();
            });
        }   
    }
    public class QueryAction {
        private DataServiceQuery<Customers> query;

        private Action<Customers> action;

        public QueryAction(DataServiceQuery<Customers> query, Action<Customers> action) {
            this.query = query;
            this.action = action;
        }

        public DataServiceQuery<Customers> Query {
            get { return query; }
        }

        public Action<Customers> Action {
            get { return action; }
        }
    }
}
