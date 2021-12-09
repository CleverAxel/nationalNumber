using System;
using LibNatNumber;

namespace nationalNumber {
    internal class MainProgram {
        static void Main(string[] args) {

            //CORRECT
            NationalNumber.IsValid("42.01.22.051-81");

            NationalNumber.IsValid("17 07 30 033 84");

            NationalNumber.IsValid("00 00 01 123 41");

            //INCORRECT
            //NationalNumber.IsValid("44 02 29 033 84");
        }
    }
}
