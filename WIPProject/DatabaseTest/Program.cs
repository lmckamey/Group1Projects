using System;

namespace DatabaseTest {
    class Program {
        static void Main(string[] args) {
            //DatabaseConnection.CheckLogin("Flameo326", "");

            CalculatorClient client = new CalculatorClient();

            string errCode = client.GetConnenctionToDB();
            if(errCode != null && errCode.Length > 0) {
                Console.WriteLine(errCode);
            }

            errCode = client.CheckLogin("Flameo326", "IAmAPotato");
            if (errCode != null && errCode.Length > 0) {
                Console.WriteLine(errCode);
            }

            errCode = client.CheckLogin("Flameo326", "IamAPotato");
            if (errCode != null && errCode.Length > 0) {
                Console.WriteLine(errCode);
            }
            client.Close();
        }
    }
}