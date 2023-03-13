namespace BlazorServerAuthenticationAndAuthorization.Data
{
    public class DBItems
    {
        public int Id { get; set; }

        //-------------------------------------------
        // Клиенты
        //-------------------------------------------
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? CarStateNumber { get; set; }
        public string? CarBrand { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }

        //-------------------------------------------
        // Клиенты/Услуги
        //-------------------------------------------
        public int IdClient { get; set; }
        public int IdOrder { get; set; }
        public int IdMaterial { get; set; }
        public string? DateOfReceipt { get; set; }
        public string? DateOfCompletion { get; set; }

        //-------------------------------------------
        // Материалы
        //-------------------------------------------
        public string? MaterialName { get; set; }
        public string? MaterialCost { get; set; }

        //-------------------------------------------
        // Услуги
        //-------------------------------------------
        public string? OrderName { get; set; }
        public int OrderPrice { get; set; }
        public int Guarantee { get; set; }
    }
}
