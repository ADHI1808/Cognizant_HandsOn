using System;
using System.Windows.Forms;
using Confluent.Kafka;
using System.Threading.Tasks;

namespace Consumer
{
    public partial class Form1 : Form
    {
        private ConsumerConfig _consumerConfig;
        private IConsumer<Ignore, string> _consumer;

        private const string KafkaBootstrapServers = "localhost:9092";
        private const string KafkaTopic = "ChatApp";
        private const string KafkaConsumerGroupId = "my_consumer_group";

        public Form1()
        {
            InitializeComponent();
            InitializeKafkaConsumer();
        }

        private void InitializeKafkaConsumer()
        {
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = KafkaBootstrapServers,
                GroupId = KafkaConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig)
                    .SetErrorHandler((consumer, err) =>
                    {
                        if (err.IsFatal)
                        {
                            MessageBox.Show($"Fatal Kafka error: {err.Reason}. Consumer will stop.", "Kafka Consumer Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Console.WriteLine($"Kafka Consumer Error: {err.Reason}");
                        }
                    })
                    .SetLogHandler((consumer, log) =>
                    {
                    })
                    .Build();

                _consumer.Subscribe(KafkaTopic);

                Task.Run(() => ConsumeMessages());

                lblStatus.Text = $"Listening to topic: {KafkaTopic}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize Kafka consumer: {ex.Message}", "Consumer Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Consumer initialization failed.";
            }
        }

        private void ConsumeMessages()
        {
            try
            {
                while (_consumer != null)
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult.IsPartitionEOF)
                    {
                        Console.WriteLine($"Reached end of topic {consumeResult.TopicPartitionOffset}.");
                        continue;
                    }

                    if (consumeResult.Message != null)
                    {
                        string receivedMessage = consumeResult.Message.Value;

                        if (txtReceivedMessages.InvokeRequired)
                        {
                            txtReceivedMessages.Invoke(new Action(() => AddMessageToDisplay(receivedMessage)));
                        }
                        else
                        {
                            AddMessageToDisplay(receivedMessage);
                        }
                    }
                }
            }
            catch (ConsumeException ex)
            {
                MessageBox.Show($"Consume error: {ex.Error.Reason}", "Kafka Consume Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during consumption: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMessageToDisplay(string message)
        {
            txtReceivedMessages.AppendText($"Received: {message}{Environment.NewLine}");
            txtReceivedMessages.ScrollToCaret();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                _consumer?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing Kafka consumer: {ex.Message}");
            }
            finally
            {
                _consumer?.Dispose();
                _consumer = null;
            }

            base.OnFormClosed(e);
        }
    }
}
