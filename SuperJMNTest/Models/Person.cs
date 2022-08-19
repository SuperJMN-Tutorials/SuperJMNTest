using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJMNTest.Models
{
    public class Person
    {
        public Person((string nombre, string apellido, int edad) person)
        {
            Nombre = person.nombre;
            Apellido = person.apellido;
            Edad = person.edad;
        }

        public string Nombre { get; }
        public string Apellido { get; }
        public int Edad { get; }

        public static Person Empty => new(("John", "Doe", 21));
    }
}
