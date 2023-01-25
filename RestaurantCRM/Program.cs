using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantCRM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (CrmServiceClient svcClient = new CrmServiceClient(ConfigurationManager.ConnectionStrings["Restaurantconnection"].ConnectionString))
            {
                if (!svcClient.IsReady)
                {
                    Console.WriteLine("unable to connect!");
                    return;
                }
                else
                {
                    WhoAmIRequest request = new WhoAmIRequest();
                    WhoAmIResponse response = (WhoAmIResponse)svcClient.Execute(request);
                    Console.WriteLine("Dynamic CRM 365 Connected....");
                    Console.WriteLine("UserId =" + response.UserId.ToString());
                    PerformCRUD(svcClient);
                    return;
                }                

            }
        }
        public static void PerformCRUD(CrmServiceClient svc)
        {
            //CREATE
            var myContact = new Entity("Customer");
            myContact.Attributes["LastName"] = "Suyambu";
            myContact.Attributes["FirstName"] = "Vivekanand";
            myContact.Attributes["Mobile"] = "9765678987";
            Guid RecordID = svc.Create(myContact);
            Console.WriteLine("Contact create with ID - " + RecordID);

            //RETRIEVE  
            Entity contact = svc.Retrieve("Customer", RecordID, new ColumnSet("FirstName", "LastName"));
            Console.WriteLine("Customer Lastname is - " + contact.Attributes["LastName"]);

            //Retrieve Multiple Record
            QueryExpression qe = new QueryExpression("Customer");
            qe.ColumnSet = new ColumnSet("FirstName", "LastName");
            EntityCollection ec = svc.RetrieveMultiple(qe);

            for (int i = 0; i < ec.Entities.Count; i++)
            {
                if (ec.Entities[i].Attributes.ContainsKey("FirstName"))
                {
                    Console.WriteLine(ec.Entities[i].Attributes["FirstName"]);
                }
            }
            Console.WriteLine("Retrieved all Customer ...");

            //UPDATE
            Entity entContact = new Entity("Customer");
            entContact.Id = RecordID;
            entContact.Attributes["LastName"] = "S";
            svc.Update(entContact);
            Console.WriteLine("Customer lastname updated");


            //DELETE
            svc.Delete("Customer", RecordID);

            //Execute
            Entity acc = new Entity("Product");
            acc["Productname"] = "Laptop";
            acc["ProductQuantity"] = 1;
            acc["ProductPrice"] = 5000;
            var createRequest = new CreateRequest()
            {
                Target = acc
            };
            svc.Execute(createRequest);


            //Execute Multiple
            var request = new ExecuteMultipleRequest()
            {
                Requests = new OrganizationRequestCollection(),
                Settings = new ExecuteMultipleSettings
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                }
            };

            Entity Pro1 = new Entity("Product");
            Pro1["Productname"] = "Keyboard";
            Pro1["ProductQuantity"] = 1;
            Pro1["ProductPrice"] = 500;
            Entity Pro2 = new Entity("Product");
            Pro2["Productname"] = "Mouse";
            Pro1["ProductQuantity"] = 1;
            Pro1["ProductPrice"] = 400;

            var createRequest1 = new CreateRequest()
            {
                Target = Pro1
            };
            var createRequest2 = new CreateRequest()
            {
                Target = Pro2
            };

            request.Requests.Add(createRequest1);
            request.Requests.Add(createRequest2);
            var response = (ExecuteMultipleResponse)svc.Execute(request);

        }
    }
}
