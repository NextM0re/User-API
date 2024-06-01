using User_API.DataObjects;

namespace User_API.Services
{
    public abstract class Validation
    {
        private static readonly char[] ForbiddenChars = { '\\', '/', '\'', '\"', ';', ':', ' ' };

        private static bool ValidateInput(string input, out string errorMessage, string inputType)
        {
            if (input.Any(ch => ForbiddenChars.Contains(ch)))
            {
                errorMessage = $"{inputType} contains forbidden characters such as {string.Join(", ", ForbiddenChars)}.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        public static bool ValidateDto(object dto, out string errorMessage)
        {
            errorMessage = string.Empty;

            var properties = dto.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(dto) as string;

                if (value != null)
                {
                    switch (property.Name)
                    {
                        case ("UserName"):
                            if (!ValidateInput(value, out errorMessage, "UserName"))
                                return false;
                            break;

                        case ("Name"):
                            if (!ValidateInput(value, out errorMessage, "Password"))
                                return false;
                            break;

                        case ("Password"):
                            if (!ValidateInput(value, out errorMessage, "Name"))
                                return false;
                            break;
                        
                        case ("Gender"):
                            var gender = int.Parse(value);
                            
                            if (gender is > 2 or < 0)
                            {
                                errorMessage = "Gender contains forbidden value. Must be either 0, 1 or 2!";
                                return false;
                            }
                            
                            break;
                    }
                }
            }

            return true;
        }
    }
}
