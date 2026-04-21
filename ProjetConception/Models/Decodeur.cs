using System.Collections.Generic;

namespace ProjetConception.Models
{
    public class Decodeur
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public int ClientId { get; set; }
        public string Address { get; set; }
        public DecoderStatus Status { get; set; }
        public List<string> Channels { get; set; }

        public Decodeur(int id, string serialNumber, int clientId, string address)
        {
            Id = id;
            SerialNumber = serialNumber;
            ClientId = clientId;
            Address = address;
            Status = DecoderStatus.Online;
            Channels = new List<string>();
        }

        public bool AddChannel(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
                return false;

            if (Channels.Contains(channel))
                return false;

            Channels.Add(channel);
            return true;
        }

        public bool RemoveChannel(string channel)
        {
            return Channels.Remove(channel);
        }
    }
}