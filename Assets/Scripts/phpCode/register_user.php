<?php
include"connect.php";

// Met hulp van https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html

//GET gets from URL
$username = $_POST["name"];
$mailadress = $_POST["mailadress"];
$password = $_POST["password"];
$birthdate = $_POST["birth_date"];
$anonymous = $_POST["anonymous"];

echo $password;

$registrationFailed = false;

//Username already exists check and email exists check from https://stackhowto.com/how-to-check-if-username-already-exists-in-database-using-php-mysql/
$select = mysqli_query($mysqli, "SELECT * FROM users WHERE name = '".$_POST['name']."'");
if(mysqli_num_rows($select)) 
{
    $registrationFailed = true;
    echo "This username already exists.\n";
}

//Check if mailadress is already in use.
$select = mysqli_query($mysqli, "SELECT * FROM users WHERE mailadress = '".$_POST['mailadress']."'");
if(mysqli_num_rows($select)) 
{
    $registrationFailed = true;
    echo "This emailadress is already in use.\n";
}

//Year 1900 based on the oldest living person on earth being born on 1904
if(strtotime($birthdate) > time() || //If you're born after today
   strtotime("1900-01-01") > strtotime($birthdate) || //If you're older than 122
   !validateDate($birthdate, 'Y-m-d')) //If your date is something like 31st of februari
{
    $registrationFailed = true;
    echo "Please enter a valid birthdate.\n";
}

//Check for username length
if(strlen($username) < 2 || strlen($username) > 40)
{
    $registrationFailed = true;
    echo "Please choose a username between 2 and 40 characters. \n";
}

//Check for password length
if(strlen($password) < 4)
{
    $registrationFailed = true;
    echo "Please choose a password greater than 3 characters.\n";
}

//Check for spaces in password
if(strpos($password,' '))
{
    $registrationFailed = true;
    echo "Please don't use spaces in your password.\n";
}

//If it passed all checks and registration hasn't failed
if(!$registrationFailed)
{
    // Hash the password using SHA-256
    $hashedPassword = hash('sha256', $password);


    $query = "INSERT INTO users (id,    name,        mailadress,    password,  reg_date, birth_date, anonymous) 
              VALUES            (NULL, '$username', '$mailadress','$hashedPassword', CURRENT_TIMESTAMP, '$birthdate', '$anonymous')";
    
    if(mysqli_query($mysqli, $query)) {
        echo "Account created successfully.";
    } 
    else 
    {
        echo "Error: " . mysqli_error($mysqli);
    }
}

//This function is from https://www.php.net/manual/en/function.checkdate.php
function validateDate($date, $format = 'Y-m-d H:i:s')
{
    $d = DateTime::createFromFormat($format, $date);
    return $d && $d->format($format) == $date;
}

//https://studentdav.hku.nl/~joris.derks/networking/register_user.php
$mysqli->close();
?>
