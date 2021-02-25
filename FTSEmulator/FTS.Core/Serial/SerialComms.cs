﻿using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTS.Core
{
    public class SerialComms : ISerial
    {
        public SerialPort Serial { get; }
        Configuration Config;
        public bool IsOpen { get; set; }

        public IDriver Driver { get; }

        public event OnTryConnect TryConnect;
        public event OnConnectSuccessful ConnectSuccessful;
        public event OnConnectFailure ConnectFailure;

        public event OnEngravingToggle EngravingToggle;

        public SerialComms(IDriver Driver = null)
        {
            Config = Configuration.Instance;

            // TODO
            // I do not understand what would vary between drivers.
            // Hence, added support so it'll be implemented soon.
            if (Driver is null) Driver = new A4988();

            Serial = new SerialPort(Config.SerialCOMPort,
                                    Config.SerialBaudRate,
                                    Config.SerialParity,
                                    Config.SerialDataBits,
                                    Config.SerialStopBits);
        }
        public bool Open()
        {
            // This connects the software to the table microcontroller
            // it works by status events OR bool return to say it's alright or not.
            return IsOpen = connect();
        }
        private bool connect()
        {
            int currentTry = 1;

            do
            {
                try
                {
                    TryConnect?.Invoke(new SerialConnectEventArgs(currentTry));
                    Serial.Open();
                    ConnectSuccessful?.Invoke(new SerialConnectEventArgs());
                    break;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    currentTry++;
                    if (currentTry == Config.SerialMaxCnnTries)
                    {
                        ConnectFailure?.Invoke(new SerialConnectEventArgs(ex, true));
                    }
                    else
                    {
                        ConnectFailure?.Invoke(new SerialConnectEventArgs(ex));
                    }
                }
            } while (currentTry < Config.SerialMaxCnnTries);

            if (Serial.IsOpen) return true;
            else return false;
        }
        // Blink por enquanto
        public void Move(Step x, Step y, Step z)
        {
            try
            {
                // this stream shouldn't be disposed!
                var bw = new BinaryWriter(Serial.BaseStream);

                byte bX = (byte)x;
                byte bY = (byte)y;
                byte bZ = (byte)z;

                var b = (bX << 4) +
                        (bY << 2) +
                         bZ;

                // TODO isso ta errado. Eu não entendi direito como devo sempre piscar 
                // uma porta pra passo e outra pra direção (aceso = direita, apagado = esquerda, por exemplo)

                // first, the binaryWriter has to write current instruction
                // That means turning given pins on.
                bw.Write((byte)b);
                bw.Flush();

                // then wait a tad... 
                Thread.Sleep(1);

                //... followed by powering down the pins
                bw.Write((byte)0x00);
                bw.Flush();
            }
            catch 
            {
                Memory.Instance.SetEmergency(EmergencyReasons.ConnectionLost);
            }
        }

        public async void ListenAsync()
        {
            // this stream mustn't be disposed!
            var sr = new BinaryReader(Serial.BaseStream, encoding: Encoding.ASCII);

            int len;
            var buffer = new byte[512];

            while (true)
            {
                try
                {
                    await Task.Delay(10);
                    if ((len = sr.Read(buffer, 0, buffer.Length)) == 0) continue;

                    var dados = Encoding.ASCII.GetString(buffer, 0, len);

                    if (dados.Contains("STOP"))
                    {
                        EngravingToggle?.Invoke(new SerialCallBackEventArgs(true));
                    }
                    else
                    {
                        EngravingToggle?.Invoke(new SerialCallBackEventArgs(false));
                    }
                }
                catch { }
            }
        }
    }
}
