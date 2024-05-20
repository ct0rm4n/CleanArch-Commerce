
using Core.ViewModel.Address;
using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Core.ViewModel.Customer
{
    public class UserVM : IBaseVM
    {
        public UserVM() 
        { 
        
        }
        [Required(ErrorMessage = "Post Name is required")]
        [StringLength(int.MaxValue, MinimumLength = 7)]
        [RegularExpression(@"^(?:.*[a-z]){7,}$", ErrorMessage = "String length must be greater than or equal 7 characters.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }


        [EmailAddress, Required]
        public string Email { get; set; }
        public bool IsValidEmail { get; set; }
        /// <summary>
        /// Gets or sets the customer Phones Number
        /// </summary>
        public string FirstPhone { get; set; }
        public string SecondaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the customer Address and default address
        /// </summary>
        public int addressId { get; set; }
        public virtual IList<AddressVM> address { get; set; }

        /// <summary>
        /// Gets or sets the customer Documents and user validators
        /// </summary>
        public virtual CpfVM customerCpf { get; set; }

        /// <summary>
        /// Gets customer fullName
        /// </summary>
        public string GetFullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }
    }
    public class CpfVM
    {
        public string CurrentCpf;
        public bool IsValidCpf;

        public CpfVM(string cpfRaw)
        {
            CurrentCpf = CpfRawToValid(cpfRaw);
            if (string.IsNullOrEmpty(CurrentCpf))
            {
                IsValidCpf = false;
            }
            else
            {
                IsValidCpf = (!CpfInput(CurrentCpf) ? true : false);
            }
        }
        private string CpfRawToValid(string cpfRaw)
        {
            cpfRaw = Regex.Replace(cpfRaw, "[^a-zA-Z0-9 ]", "").Substring(0, 11);
            cpfRaw = Convert.ToInt64(cpfRaw).ToString(@"000\.000\.000\-00");
            return cpfRaw;
        }
        private bool CpfInput(string cpfRaw)
        {
            try
            {
                var cpf = CpfRawToValid(cpfRaw);
                if (!string.IsNullOrEmpty(cpf) && cpf.Length != 14)
                    return false;

                return CpfValidate(cpfRaw);
            }
            catch (Exception)
            {
                return false;
            }

        }
        private static bool CpfValidate(string cpf)
        {
            if (String.IsNullOrWhiteSpace(cpf))
                return false;

            string clearCPF;
            clearCPF = cpf.Trim();
            clearCPF = clearCPF.Replace("-", "");
            clearCPF = clearCPF.Replace(".", "");

            if (clearCPF.Length != 11)
            {
                return false;
            }

            int[] cpfArray;
            int totalDigitoI = 0;
            int totalDigitoII = 0;
            int modI;
            int modII;

            if (clearCPF.Equals("00000000000") ||
                clearCPF.Equals("11111111111") ||
                clearCPF.Equals("22222222222") ||
                clearCPF.Equals("33333333333") ||
                clearCPF.Equals("44444444444") ||
                clearCPF.Equals("55555555555") ||
                clearCPF.Equals("66666666666") ||
                clearCPF.Equals("77777777777") ||
                clearCPF.Equals("88888888888") ||
                clearCPF.Equals("99999999999"))
            {
                return false;
            }

            foreach (char c in clearCPF)
            {
                if (!char.IsNumber(c))
                {
                    return false;
                }
            }

            cpfArray = new int[11];
            for (int i = 0; i < clearCPF.Length; i++)
            {
                cpfArray[i] = int.Parse(clearCPF[i].ToString());
            }

            for (int posicao = 0; posicao < cpfArray.Length - 2; posicao++)
            {
                totalDigitoI += cpfArray[posicao] * (10 - posicao);
                totalDigitoII += cpfArray[posicao] * (11 - posicao);
            }

            modI = totalDigitoI % 11;
            if (modI < 2) { modI = 0; }
            else { modI = 11 - modI; }

            if (cpfArray[9] != modI)
            {
                return false;
            }

            totalDigitoII += modI * 2;

            modII = totalDigitoII % 11;
            if (modII < 2) { modII = 0; }
            else { modII = 11 - modII; }
            if (cpfArray[10] != modII)
            {
                return false;
            }
            // CPF Válido!
            return true;
        }

    }

}
