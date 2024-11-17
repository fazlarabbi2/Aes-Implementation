namespace AESServerAPP.Models
{
    public class KeyManagementService
    {
        public static List<ClientKeyIV> listKeyIvs = new List<ClientKeyIV>()
        {
            new ClientKeyIV() { ClientId = "DefaultClient", Key = "Yyj9nVLtBLwPANTqZNFHrofcH/AbvJlaUbytoHT8Qd8=", IV = "/X9EAc4vBALd31ye7N3L1g=="},
            new ClientKeyIV() { ClientId = "Client1", Key = "gi1D2eDd8Tg565ZbfRWc00j9xKtBka4ZHu0Sen+Drgc=", IV = "Qb4nTgWS7UBo2YU7G/gJCg=="},
            new ClientKeyIV() { ClientId = "Client2", Key = "mPjeDLj4jq5AnX/0WeDXBewm05AIOqbV83MfNTWap7A=", IV = "3Y/S5SC3qFNaSbfSKEKxxA=="},
            new ClientKeyIV() { ClientId = "Client3", Key = "J0N55pEAha+B0Oyggc4zWV1GE9iWiW/m7W5DuUo0W3M=", IV = "8PiYfRaj4e5JumnpLh0FzA=="}
        };

        public static ClientKeyIV? GetKeyAndIV(string clientId)
        {
            return listKeyIvs.FirstOrDefault(i => i.ClientId.ToLower() == clientId.ToLower());
        }
    }

    public class ClientKeyIV
    {
        public string ClientId { get; set; }
        public string Key { get; set; }
        public string IV { get; set; }
    }
}
