<?php
include "connect.php";

$username = $_POST["username"];
$password = $_POST["password"];

if (empty($username) || empty($password)) {
    echo "1";
    exit();
}

// Hash the password using SHA-256
$hashedPassword = hash('sha256', $password);

$query = "SELECT id, name, mailadress, reg_date, birth_date FROM users WHERE name = '$username' AND password = '$hashedPassword'";

if (!($result = $mysqli->query($query))) {
    showerror($mysqli->errno, $mysqli->error);
} 

if ($result->num_rows == 1) {
    $row = $result->fetch_assoc();
    $userdata['id'] = $row["id"];
    $userdata['name'] = $username;
    $userdata['mailadress'] = $row["mailadress"];
    $userdata['reg_date'] = strtotime($row["reg_date"]);
    $userdata['birth_date'] = strtotime($row["birth_date"]);
    echo json_encode($userdata);
} 
else {
    echo "0";
}

$mysqli->close();
?>
