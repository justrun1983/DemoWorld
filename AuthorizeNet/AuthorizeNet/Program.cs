using System;
using AuthorizeNet;

namespace AuthorizeNetDemo
{
    class Program
    {
        private const string ApiKey = "8Ns4Ue6Q";
        private const string TransactionKey = "7m575Z596zP52yBN";

        private static void Main(string[] args)
        {
            AddCustomer();
            AimPaySimpleDemo();
            AimPayComplexDemo();
            AddCustomerWithPaymentProfile();
            Console.Read();
        }


        /// <summary>
        /// The AIM payment demo
        /// </summary>
        public static void AimPaySimpleDemo()
        {
            // Step 1 - Create the request
            var request = new AuthorizationRequest("4111111111111111", "1216", 10.00M, "Test Transaction");

            // Step 2 - Create the gateway, sending in your credentials
            var gate = new Gateway(ApiKey, TransactionKey, true);

            // Step 3 - Send the request to the gateway
            var response = gate.Send(request);

            Console.WriteLine("{0}: {1}", response.ResponseCode, response.Message);
        }

        public static void AimPayComplexDemo()
        {
            // Step 1 - Create the request
            var request = new AuthorizationRequest("4111111111111111", "1216", 10.00M, "Test Transaction");
            request.Queue("x_cust_id", "29816378");
            request.Queue("x_customer_ip", "192.168.1.1");
            request.Queue("x_first_name", "peter");
            request.Queue("x_last_name", "qi");
            request.Queue("x_invoice_num", "27066093");

            // Step 2 - Create the gateway, sending in your credentials
            var gate = new Gateway(ApiKey, TransactionKey, true);

            // Step 3 - Send the request to the gateway
            var response = gate.Send(request);

            Console.WriteLine("{0}: {1}", response.ResponseCode, response.Message);
        }

            
        /// <summary>
        /// The CIM API to add customer
        /// </summary>
        private static void AddCustomer()
        {
            var target = new CustomerGateway(ApiKey, TransactionKey);

            try
            {
                var actual = target.CreateCustomer("john@doe.com", "new customer profile");
            }
            catch (Exception e)
            {
              Console.WriteLine(e.Message); 
            }
        }

        private static void AddCustomerWithPaymentProfile()
        {
            var target = new CustomerGateway(ApiKey, TransactionKey);

            try
            {
                //var s = target.GetCustomerIDs();
                var actual = target.CreateCustomer("john@doe.com", "new customer profile", "102");
                var profileId = actual.ProfileID;
                target.AddCreditCard(profileId, "4111111111111111", 12, 16, "087", new Address
                                                                                   {
                                                                                       City = "WeiHai",
                                                                                       State = "Shangdong",
                                                                                       Zip = "264209",
                                                                                       Street = "Touch Road"
                                                                                   });
            }
            catch (Exception e)
            {
              Console.WriteLine(e.Message); 
            } 
        }
    }
}
