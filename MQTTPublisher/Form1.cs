using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace MQTTPublisher
{
    public partial class Form1 : Form
    {
        IMqttClient client;
        public Form1()
        {
            InitializeComponent();
        }

        void btnPublish_Click(object sender, EventArgs e)
        {
            if (client==null)
            {
                Connect();
            }
            else
            {
                if (!client.IsConnected)  Connect();              
            }
            Publish();
        }


        async Task Connect()
        {
            //var server = "test.mosquitto.org";
            //server= "broker.hivemq.com";
            var server = "localhost";
            var port = 1883;
            var mqttFactory = new MqttFactory();
            client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(server, port)
                .WithCleanSession()
                .Build();
            client.ConnectedAsync += Client_ConnectedAsync;
            client.DisconnectedAsync += Client_DisconnectedAsync;
            await client.ConnectAsync(options);
            string msg = "connect,server=" +server+",port="+ port.ToString();
            WriteLog(msg);
        }

        async Task Publish()
        {
            var msg = txtMsg.Text;
            var userId = txtUser.Text;
            var topic = "mqtt/pbx/topic/" + userId;
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();            
            if (client.IsConnected)
            {
                await client.PublishAsync(message);
            }
            var doneMsg = "message published,topic="+topic+",msg="+msg ;
            WriteLog(doneMsg);
        }


        private Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            var msg="Disconnected from broker!";
            WriteLog(msg);
            return Task.CompletedTask;
        }

        private Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            var msg = "connected to the broker! ";
            WriteLog(msg);
            return Task.CompletedTask;  
        }

        void WriteLog(string msg)
        {
            var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            txtLog.Invoke((MethodInvoker)(() =>
            {
                txtLog.Text = date + "\t" + msg + "\r\n" + txtLog.Text;
            }));
        }

        

    }
}