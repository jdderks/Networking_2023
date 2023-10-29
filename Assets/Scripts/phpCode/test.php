<?php
include "connect.php";

if (!$mysqli) {
    die("Connection failed: " . mysqli_connect_error());
}

$result = mysqli_query($mysqli, "SELECT mailadress FROM users");

if ($result) {
    while ($row = mysqli_fetch_assoc($result)) {
        echo $row['mailadress'] . "<br>";
    }
    mysqli_free_result($result);
} else {
    echo "Error: " . mysqli_error($mysqli);
}

mysqli_close($mysqli);
?>
