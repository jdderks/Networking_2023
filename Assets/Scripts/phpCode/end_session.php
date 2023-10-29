<?php
include "connect.php";

$session_name = $_POST["session_name"];

if (empty($session_name)) {
    echo "Invalid session name";
    exit();
}

$updateQuery = "UPDATE sessions SET is_active = 0 WHERE session_name = '$session_name'";

if (!$mysqli->query($updateQuery)) {
    showerror($mysqli->errno, $mysqli->error);
} else {
    echo "Session '$session_name' is now inactive";
}

$mysqli->close();
?>
