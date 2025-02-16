using System.Threading.Tasks;
using Data.Models;

namespace Business.Interfaces
{
    public interface IUserService
    {
        // Εκτελεί τη διαδικασία εγγραφής ενός νέου χρήστη
        // Λαμβάνει ένα αντικείμενο UserRegisterDto που περιέχει τα στοιχεία του χρήστη και επιστρέφει ένα αντικείμενο AuthResult που περιέχει πληροφορίες σχετικά με το αποτέλεσμα της εγγραφής
        Task<AuthResult> RegisterAsync(UserRegisterDto userDto);

        // Εκτελεί τη διαδικασία σύνδεσης ενός χρήστη
        // Λαμβάνει ένα αντικείμενο UserLoginDto που περιέχει τα στοιχεία σύνδεσης και επιστρέφει ένα αντικείμενο AuthResult που περιέχει το αποτέλεσμα της προσπάθειας σύνδεσης
        Task<AuthResult> LoginAsync(UserLoginDto loginDto);
    }

}