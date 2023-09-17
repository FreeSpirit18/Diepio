using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Server
{
    internal class ClassHub
    {
        HubConnection _connection;
        string _server;

        public ClassHub(string server)
        {
            _server = server;
        }

        public async void Connect()
        {
            _connection = new HubConnectionBuilder().WithUrl(_server).Build();
            await _connection.StartAsync();
            _connection.On("GetNotification", (string userCount) =>
            {
                Console.WriteLine(userCount);
            });
        }
    }
}
