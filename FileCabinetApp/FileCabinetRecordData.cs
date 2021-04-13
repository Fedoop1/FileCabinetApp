using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecordData
    {
        public short Height { get; private set; }

        public decimal Money { get; private set; }

        public char Gender { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        public void InputData()
        {
            try
            {
                Console.WriteLine("\nFirst name: ");

                this.FirstName = Console.ReadLine();

                Console.WriteLine("Last name: ");

                this.LastName = Console.ReadLine();

                Console.WriteLine("Date of birth: ");

                if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
                {
                    throw new ArgumentException("Date of birth is incorrect.");
                }

                this.DateOfBirth = dateOfBirth;
                Console.WriteLine("Height: ");

                if (!short.TryParse(Console.ReadLine(), out short height))
                {
                    throw new ArgumentException("Height is incorrect");
                }

                this.Height = height;
                Console.WriteLine("Money: ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal money))
                {
                    throw new ArgumentException("Money is incorrect");
                }

                this.Money = money;
                Console.WriteLine("Gender(M/F): ");

                if (!char.TryParse(Console.ReadLine(), out char gender))
                {
                    throw new ArgumentException("Gender is incorrect");
                }

                this.Gender = gender;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
                this.InputData();
            }
        }
    }
}
