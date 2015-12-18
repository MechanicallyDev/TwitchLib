﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib;

namespace TwitchLibExample
{
    public partial class Form1 : Form
    {
        List<TwitchChatClient> chatClients = new List<TwitchChatClient>();
        List<TwitchWhisperClient> whisperClients = new List<TwitchWhisperClient>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            MessageBox.Show("This application is intended to demonstrate basic functionality of TwitchLib.\n\n-swiftyspiffy");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(ConnectionCredentials.ClientType.CHAT, new TwitchIpAndPort(textBox8.Text, true), 
                textBox4.Text, textBox5.Text);
            TwitchChatClient newClient = new TwitchChatClient(textBox8.Text, credentials);
            newClient.NewChatMessage += new EventHandler<TwitchChatClient.NewChatMessageArgs>(globalChatMessageReceived);
            newClient.CommandReceived += new EventHandler<TwitchChatClient.CommandReceivedArgs>(commandReceived);
            newClient.connect();
            chatClients.Add(newClient);
            ListViewItem lvi = new ListViewItem();
            lvi.Text = textBox4.Text;
            lvi.SubItems.Add("CHAT");
            lvi.SubItems.Add(textBox8.Text);
            listView1.Items.Add(lvi);
            if(!comboBox2.Items.Contains(textBox4.Text))
                comboBox2.Items.Add(textBox4.Text);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(ConnectionCredentials.ClientType.WHISPER, new TwitchIpAndPort(true),
                textBox6.Text, textBox7.Text);
            TwitchWhisperClient newClient = new TwitchWhisperClient(credentials);
            newClient.NewWhisper += new EventHandler<TwitchWhisperClient.NewWhisperReceivedArgs>(globalWhisperReceived);
            newClient.connect();
            whisperClients.Add(newClient);
            ListViewItem lvi = new ListViewItem();
            lvi.Text = textBox6.Text;
            lvi.SubItems.Add("WHISPER");
            lvi.SubItems.Add("N/A");
            listView1.Items.Add(lvi);
            comboBox1.Items.Add(textBox6.Text);
        }

        private void commandReceived(object sender, TwitchChatClient.CommandReceivedArgs e)
        {
            listBox1.Items.Add(e.username + ": " + e.argumentsAsString);
        }

        private void globalChatMessageReceived(object sender, TwitchChatClient.NewChatMessageArgs e)
        {
            //Don't do this in production
            CheckForIllegalCrossThreadCalls = false;

            richTextBox1.Text = String.Format("#{0} {1}: {2}", e.ChatMessage.Channel, e.ChatMessage.DisplayName, e.ChatMessage.Message) + 
                "\n" + richTextBox1.Text;
        }

        private void globalWhisperReceived(object sender, TwitchWhisperClient.NewWhisperReceivedArgs e)
        {
            //Don't do this in production
            CheckForIllegalCrossThreadCalls = false;

            richTextBox2.Text = String.Format("{0} -> {1}: {2}", e.WhisperMessage.Username, e.WhisperMessage.BotUsername, e.WhisperMessage.Message) + 
                "\n" + richTextBox2.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            foreach (TwitchChatClient client in chatClients)
            {
                if(client.TwitchUsername.ToLower() == comboBox2.Text.ToLower()) {
                    comboBox3.Items.Add(client.Channel);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (TwitchChatClient client in chatClients)
            {
                if (client.TwitchUsername.ToLower() == comboBox2.Text.ToLower())
                {
                    if (client.Channel.ToLower() == comboBox3.Text.ToLower())
                    {
                        client.sendMessage(textBox3.Text);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (TwitchWhisperClient client in whisperClients)
            {
                if (client.TwitchUsername == comboBox1.Text.ToLower())
                {
                    client.sendWhisper(textBox1.Text, textBox2.Text);
                    Console.WriteLine("fired");
                }
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            TwitchChannel channel = await TwitchAPI.getTwitchChannel(textBox9.Text);
            MessageBox.Show(String.Format("Status: {0}\nBroadcaster Lang: {1}\nDisplay Name: {2}\nGame: {3}\nLanguage: {4}\nName: {5}\nCreated At: {6}\n" +
                "Updated At: {7}\nDelay: {8}\nLogo: {9}\nBackground: {10}\nProfile Banner: {11}\nMature: {12}\nPartner: {13}\nID: {14}\nViews: {15}\nFollowers: {16}",
                channel.Status, channel.Broadcaster_Language, channel.Display_name, channel.Game, channel.Language, channel.Name, channel.Created_At, channel.Updated_At,
                channel.Delay, channel.Logo, channel.Background, channel.Profile_Banner, channel.Mature, channel.Partner, channel.ID, channel.Views, channel.Followers));
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            List<Chatter> chatters = await TwitchAPI.getChatters(textBox10.Text);
            string messageContents = "";
            foreach(Chatter user in chatters)
            {
                if(messageContents == "")
                {
                    messageContents = String.Format("{0} ({1})", user.Username, user.UserType.ToString());
                } else
                {
                    messageContents += String.Format(", {0} ({1})", user.Username, user.UserType.ToString());
                }
            }
            MessageBox.Show(messageContents);
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            if (await TwitchAPI.userFollowsChannel(textBox11.Text, textBox12.Text))
            {
                MessageBox.Show(String.Format("'{0}' follows the channel '{1}'!", textBox11.Text, textBox12.Text));
            } else
            {
                MessageBox.Show(String.Format("'{0}' does NOT follow the channel '{1}'!", textBox11.Text, textBox12.Text));
            }   
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            if(await TwitchAPI.broadcasterOnline(textBox13.Text))
            {
                MessageBox.Show(String.Format("'{0}' is ONLINE!", textBox13.Text));
            } else
            {
                MessageBox.Show(string.Format("'{0}' is OFFLINE!", textBox13.Text));
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TwitchAPI.updateStreamTitle(textBox16.Text, textBox14.Text, textBox15.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TwitchAPI.updateStreamGame(textBox17.Text, textBox14.Text, textBox15.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TwitchAPI.updateStreamTitleAndGame(textBox18.Text, textBox19.Text, textBox14.Text, textBox15.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            TwitchAPI.runCommerciale(TwitchAPI.Valid_Commercial_Lengths.SECONDS_30, textBox14.Text, textBox15.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            TwitchAPI.runCommerciale(TwitchAPI.Valid_Commercial_Lengths.SECONDS_60, textBox14.Text, textBox15.Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            TwitchAPI.runCommerciale(TwitchAPI.Valid_Commercial_Lengths.SECONDS_90, textBox14.Text, textBox15.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            TwitchAPI.resetStreamKey(textBox14.Text, textBox15.Text);
        }

        private async void button16_Click(object sender, EventArgs e)
        {
            
            List<TwitchLib.TwitchAPIClasses.TwitchVideo> videos = await TwitchAPI.getChannelVideos(textBox20.Text);
            foreach(TwitchLib.TwitchAPIClasses.TwitchVideo vid in videos)
            {
                MessageBox.Show(string.Format("Title: {0}\nDescription: {1}\nStatus: {2}\nID: {3}\nTag List: {4}\nRecorded At: {5}\nGame: {6}\nDelete At: {7}\nPreview: {8}\n" +
                    "Broadcast ID: {9}\nLength: {10}\nIs Muted: {11}\n\nURL: {12}\nViews: {13}\n\n" +
                    "FPS Audio Only: {14}\nFPS Mobile: {15}\nFPS Low: {16}\nFPS Medium: {17}\nFPS High: {18}\nFPS Chunked: {19}\n\n" +
                    "Resolution Mobile: {20}\nResolution Low: {21}\nResolution Medium: {22}\nResolution High: {23}\nResolution Chunked: {24}\n\n" +
                    "Channel Name: {25}\nChannel Display Name: {26}", vid.Title, vid.Description, vid.Status, vid.ID, vid.Tag_List, vid.Recorded_At, vid.Game, vid.Delete_At,
                    vid.Preview, vid.Broadcast_ID, vid.Length, vid.Is_Muted, vid.URL, vid.Views, vid.FPS.Audio_Only,
                    vid.FPS.Mobile, vid.FPS.Low, vid.FPS.Medium, vid.FPS.High, vid.FPS.Chunked, vid.Resolutions.Mobile, vid.Resolutions.Low,
                    vid.Resolutions.Medium, vid.Resolutions.High, vid.Resolutions.Chunked, vid.Channel.Name, vid.Channel.Display_Name));
            }
        }
    }
}
