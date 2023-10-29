<?php
include "connect.php";

$session_name = $_POST["session_name"];

if (empty($session_name)) {
    echo "Invalid session name";
    exit();
}

$connected_players = 1;

$query = "INSERT INTO sessions (session_name, session_time, connected_players, is_active) VALUES ('$session_name', CURRENT_TIMESTAMP, '$connected_players', true)";

if (!$mysqli->query($query)) {
    showerror($mysqli->errno, $mysqli->error);
} else {
    echo $session_name;
}

$mysqli->close();
?>
