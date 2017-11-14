using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ApplicationToServerService {

    //[ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    [ServiceContract]
    public interface ICalculator {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }
}
/*
 * A WCF Service functions as a host/ service and client.
 * 
 * 
 * The Service must be a WCF Service Library Project
 * The project will give your 2 files.
 * IService is where interfaces are defined. They indicate what the Client has access to.
 * 
 * A class/ interface should be declared with the [ServiceContract] attribute to be used by client.
 *  This defines a "service"
 * Methods in a service should be declared with the [OperationContract] attritube to be used by client.
 *  This defines an "operation" the client can call.
 * 
 * A class/ interface can also be defined as [DataContract] which allows the client to use and pass Data.
 *  This defined a "datatype"
 * Properties should be declared with [DataMember] and allows the client to access the data.
 * 
 * 
 * In the Service file, you define what your services do. 
 * Typically, you create a class which inherits the interfaces the define how methods work.
 * 
 * Running the WCF project will create a temp Host and Client and allows devs to test the methods.
 * 
 * 
 * 
 * To actually create a seperate host, a new project must connect to the service and define additional behaviour
 * Self Hosting, or hosting in a managed environment such as a new VS project requires the service
 *      to be wrapped in a ServiceHost
 * Windows Activation Service is the most efficient and supportive.
 * 
 * To create IIS WCF service,
 *  Create new Website
 *  Choose WCF Server as the type
 *  Set it's location to HTTP
 *  Create.
 *  ??????????????
 * 
 * To create a Self-Hosting Application,
 *  Create a console Project
 *  Add a refernece to Service Module
 *  Create a URI for the Service Location. This will typically be at: http://localhost:[portNum]/[ProjectName]/[opetional:SpecificServiceName]
 *  Create a new ServiceHost
 *  Add an Endpoint for connection
 *  Enable Metadata
 *  Then Activate the Service
 *  
 * To create a Windows Service Host
 *  Create a class library
 *  Add a reference to ServiceModel
 *  Add Services and classes for the service
 *  Build to get the DLL
 */
