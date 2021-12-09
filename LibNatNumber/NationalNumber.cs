using System;
using System.Text;

namespace LibNatNumber {
    public class NationalNumber {
        /// <summary>
        /// Retourne vrai si le numéro est valide.
        /// </summary>
        /// <param name="pNationalNumber">Entrée du numéro nationale, doit être obligatoirement constitué de 11 chiffres</param>
        /// <returns></returns>
        public static bool IsValid(string pNationalNumber) {
            string normalizedValue = NormalizeNationalNumber(pNationalNumber);

            if (normalizedValue.Length == 11) {
                TestValid previousCentury = new TestValid(IsDateValid(1900, normalizedValue), IsOrderNumberValid(normalizedValue), IsControlNumberValid(normalizedValue, ""), 1900);
                TestValid currentCentury = new TestValid(IsDateValid(2000, normalizedValue), IsOrderNumberValid(normalizedValue), IsControlNumberValid(normalizedValue, "2"), 2000);

                if (previousCentury.IsDateValid && previousCentury.IsOrderNumberValid && previousCentury.IsControlNumberValid) {
                    Console.WriteLine("Supposé né dans les années " + previousCentury.Century + " et numéro valide.");
                    return true;
                }

                if (currentCentury.IsDateValid && currentCentury.IsOrderNumberValid && currentCentury.IsControlNumberValid) {
                    Console.WriteLine("Supposé né dans les années " + currentCentury.Century + " et numéro valide.");
                    return true;
                }

                //Exception avec comme 6 premiers chiffres 00 00 01 pour les gens sans date de naissance.
                if (IsNoBirthDateValid(normalizedValue)) {
                    Console.WriteLine("Supposé n'ayant pas de date de naissance et numéro valide");
                    return true;
                }

                DisplayError(previousCentury, currentCentury);
                return false;
            }

            Console.WriteLine("Nombre trop grand ou trop petit, il manque des informations. (Doit être composé de 11 chiffres)");
            return false;
        }

        //Normalise l'entrée ne garde que les nombres.
        private static string NormalizeNationalNumber(string pNationalNumber) {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < pNationalNumber.Length; i++) {
                if (Char.IsDigit(pNationalNumber[i])) {
                    sb.Append(pNationalNumber[i]);
                }
            }

            string normalizedValue = sb.ToString();
            return normalizedValue;
        }

        //Teste la date en fonction du siècle en cours passé via paramètre(1900, 2000 etc...)
        private static bool IsDateValid(int pCentury, string pNormalizedValue) {
            int year = int.Parse(pNormalizedValue.Substring(0, 2));
            int month = int.Parse(pNormalizedValue.Substring(2, 2));
            int day = int.Parse(pNormalizedValue.Substring(4, 2));

            year += pCentury;

            bool dateValide = true;

            try {
                DateTime date = new DateTime(year, month, day);
            }
            catch {
                dateValide = false;
            }

            if (dateValide && pCentury == 2000) {
                if (IsInTheFuture(year, month, day)) {
                    return false;
                }
            }

            return dateValide;
        }

        //Regarde si la personne est montée dans une Delorean avec un vieux.
        private static bool IsInTheFuture(int pYear, int pMonth, int pDay) {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;

            if (pYear > currentYear) {
                return true;
            }
            if (pYear == currentYear && pMonth > currentMonth) {
                return true;
            }
            if (pYear == currentYear && pMonth == currentMonth && pDay > currentDay) {
                return true;
            }

            return false;
        }

        //Regarde si les trois nombres après la date sont entre 0 et 999
        private static bool IsOrderNumberValid(string pNormalizedValue) {
            int orderNumber = int.Parse(pNormalizedValue.Substring(6, 3));

            if (orderNumber > 0 && orderNumber < 999) {
                return true;
            }

            return false;
        }

        //Regarde si le numéro de controle est valide.
        //Mettre juste un string vide au paramètre "numberToAddInFront" pour ne rien rajouter.
        //On mettra un deux à ce paramètre en question si on pense que la personne est né dans les années 2000.
        private static bool IsControlNumberValid(string pNormalizedValue, string numberToAddInFront) {
            pNormalizedValue = numberToAddInFront + pNormalizedValue;
            int currentNumber = 0;

            string otherInfoToCalcul = "";
            while (currentNumber < pNormalizedValue.Length - 2) {
                otherInfoToCalcul += pNormalizedValue[currentNumber];
                currentNumber++;
            }

            string last2Numbers = "";
            while (currentNumber < pNormalizedValue.Length) {
                last2Numbers += pNormalizedValue[currentNumber];
                currentNumber++;
            }

            long controlNumberExpected = long.Parse(last2Numbers);
            long otherInfo = long.Parse(otherInfoToCalcul);

            long controlNumberCalculated = (otherInfo % 97) - 97;

            //Exception qui dit que si ça tombe à zéro, ça doit être à 97.
            if (controlNumberCalculated == 0) {
                controlNumberCalculated = -97;
            }

            if (controlNumberCalculated == -controlNumberExpected) {
                return true;
            }

            return false;
        }

        private static bool IsNoBirthDateValid(string pNormalizedValue) {
            string year = pNormalizedValue.Substring(0, 2);
            string month = pNormalizedValue.Substring(2, 2);
            string day = pNormalizedValue.Substring(4, 2);

            if (year == "00" && month == "00" && day == "01" && IsOrderNumberValid(pNormalizedValue) && IsControlNumberValid(pNormalizedValue, "")) {
                return true;
            }

            return false;
        }

        private static void DisplayError(TestValid pPreviousCentury, TestValid pCurrentCentury) {
            Console.WriteLine("-----ERREUR DANS L'ENCODAGE-----\n");
            if (pPreviousCentury.IsDateValid) {
                Console.WriteLine("Date plausible pour les personnes nées dans les années " + pPreviousCentury.Century);
            } else {
                Console.WriteLine("Date non-plausible pour les personnes nées dans les années " + pPreviousCentury.Century);
            }

            if (pCurrentCentury.IsDateValid) {
                Console.WriteLine("Date plausible pour les personnes nées dans les années " + pCurrentCentury.Century);
            } else {
                Console.WriteLine("Date non-plausible pour les personnes nées dans les années " + pCurrentCentury.Century);
            }

            if (pPreviousCentury.IsOrderNumberValid || pCurrentCentury.IsOrderNumberValid) {
                Console.WriteLine("Numéro d'ordre plausible.");
            } else {
                Console.WriteLine("Numéro d'ordre non-plausible.");
            }
            Console.WriteLine();
            Console.WriteLine("Le numéro de contrôle calculé avec la date et le numéro d'ordre que");
            Console.WriteLine("vous avez fournis n'est pas le même que celui que vous avez rentré.");
            Console.WriteLine("-------------------------------");
        }

        private class TestValid {
            public bool IsDateValid { get; set; }
            public bool IsOrderNumberValid { get; set; }
            public bool IsControlNumberValid { get; set; }
            public int Century { get; set; }

            public TestValid(bool isDateValid, bool isOrderNumberValid, bool isControlNumberValid, int century) {
                IsDateValid = isDateValid;
                IsOrderNumberValid = isOrderNumberValid;
                IsControlNumberValid = isControlNumberValid;
                Century = century;
            }
        }
    }
}
