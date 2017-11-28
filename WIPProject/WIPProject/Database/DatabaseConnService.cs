﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IDatabaseConn")]
public interface IDatabaseConn
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/GetConnenctionToDB", ReplyAction="http://tempuri.org/IDatabaseConn/GetConnenctionToDBResponse")]
    string GetConnenctionToDB();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/GetConnenctionToDB", ReplyAction="http://tempuri.org/IDatabaseConn/GetConnenctionToDBResponse")]
    System.Threading.Tasks.Task<string> GetConnenctionToDBAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/CheckLogin", ReplyAction="http://tempuri.org/IDatabaseConn/CheckLoginResponse")]
    string CheckLogin(string userName, string password);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/CheckLogin", ReplyAction="http://tempuri.org/IDatabaseConn/CheckLoginResponse")]
    System.Threading.Tasks.Task<string> CheckLoginAsync(string userName, string password);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/AddNewUser", ReplyAction="http://tempuri.org/IDatabaseConn/AddNewUserResponse")]
    string AddNewUser(string userName, string password);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/AddNewUser", ReplyAction="http://tempuri.org/IDatabaseConn/AddNewUserResponse")]
    System.Threading.Tasks.Task<string> AddNewUserAsync(string userName, string password);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/StartNewServer", ReplyAction="http://tempuri.org/IDatabaseConn/StartNewServerResponse")]
    void StartNewServer(string userName, string password);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDatabaseConn/StartNewServer", ReplyAction="http://tempuri.org/IDatabaseConn/StartNewServerResponse")]
    System.Threading.Tasks.Task StartNewServerAsync(string userName, string password);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IDatabaseConnChannel : IDatabaseConn, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class DatabaseConnClient : System.ServiceModel.ClientBase<IDatabaseConn>, IDatabaseConn
{
    
    public DatabaseConnClient()
    {
    }
    
    public DatabaseConnClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public DatabaseConnClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public DatabaseConnClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public DatabaseConnClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public string GetConnenctionToDB()
    {
        return base.Channel.GetConnenctionToDB();
    }
    
    public System.Threading.Tasks.Task<string> GetConnenctionToDBAsync()
    {
        return base.Channel.GetConnenctionToDBAsync();
    }
    
    public string CheckLogin(string userName, string password)
    {
        return base.Channel.CheckLogin(userName, password);
    }
    
    public System.Threading.Tasks.Task<string> CheckLoginAsync(string userName, string password)
    {
        return base.Channel.CheckLoginAsync(userName, password);
    }
    
    public string AddNewUser(string userName, string password)
    {
        return base.Channel.AddNewUser(userName, password);
    }
    
    public System.Threading.Tasks.Task<string> AddNewUserAsync(string userName, string password)
    {
        return base.Channel.AddNewUserAsync(userName, password);
    }
    
    public void StartNewServer(string userName, string password)
    {
        base.Channel.StartNewServer(userName, password);
    }
    
    public System.Threading.Tasks.Task StartNewServerAsync(string userName, string password)
    {
        return base.Channel.StartNewServerAsync(userName, password);
    }
}
