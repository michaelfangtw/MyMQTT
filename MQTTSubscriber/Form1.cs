using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace MQTTSubscriber
{
    public partial class Form1 : Form
    {
        IMqttClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect();  
        }


        async Task Connect()
        {
            //var server = "test.mosquitto.org";
            //server = "broker.hivemq.com";
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
            client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            await client.ConnectAsync(options);            
            var msg ="connect,server=" + server + ",port=" + port.ToString();
            WriteLog(msg);
        }

        private Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            var msg =$"received: {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}";
            WriteLog(msg);
            return Task.CompletedTask;
        }

        private Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            var msg = "Disconnected from broker!";
            WriteLog(msg);
            return Task.CompletedTask;
        }

        private Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            var msg = "connected to the broker!";
            WriteLog(msg);
            var userId = txtUser.Text;            
            var topic = "mqtt/pbx/topic/" + userId;
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .Build();
            client.SubscribeAsync(topicFilter);
            var subscribeMsg = "subscribe topic=" + topic;
            WriteLog(subscribeMsg);
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}