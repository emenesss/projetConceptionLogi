using System.Collections.Generic;
using System.Linq;

namespace ProjetConception.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Decodeur> Decoders { get; set; }
        public List<int> AuthorizedUserIds { get; set; }

        public Client(int id, string name)
        {
            Id = id;
            Name = name;
            Decoders = new List<Decodeur>();
            AuthorizedUserIds = new List<int>();
        }

        public void AddDecoder(Decodeur decoder)
        {
            Decoders.Add(decoder);
        }

        public bool RemoveDecoder(int decoderId)
        {
            var decoder = Decoders.FirstOrDefault(d => d.Id == decoderId);
            if (decoder == null)
                return false;

            return Decoders.Remove(decoder);
        }

        public Decodeur? FindDecoderById(int decoderId)
        {
            return Decoders.FirstOrDefault(d => d.Id == decoderId);
        }
    }
}