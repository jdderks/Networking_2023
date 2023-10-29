<?php
include "connect.php";

$session_name = $_POST["session_name"];

if (empty($session_name)) {
    echo "Invalid session name";
    exit();
}

$checkActiveQuery = "SELECT * FROM sessions WHERE session_name = '$session_name' AND is_active = 1";

if (!($result = $mysqli->query($checkActiveQuery))) {
    showerror($mysqli->errno, $mysqli->error);
}

if ($result->num_rows == 1) {
    $updatePlayersQuery = "UPDATE sessions SET connected_players = connected_players + 1 WHERE session_name = '$session_name'";

    if (!$mysqli->query($updatePlayersQuery)) {
        showerror($mysqli->errno, $mysqli->error);
    } else {
        echo "Connected players updated successfully";
    }
} else {
    echo "Session is not active";
}

$mysqli->close();
?>
