<?php
include "connect.php";

$topLastMonth = $_POST["topLastMonth"] ?? false;

$query = "SELECT 
            IF(u.anonymous = 1, 'Anonymous', m.player) as player, 
            SUM(m.turns_taken) as turns_taken 
          FROM matches m 
          LEFT JOIN users u ON m.player = u.name 
          GROUP BY player 
          ORDER BY turns_taken ASC 
          LIMIT 5";

if ($topLastMonth) {
    $query = "
        SELECT 
            IF(u.anonymous = 1, 'Anonymous', m.player) as player, 
            SUM(m.turns_taken) as turns_taken 
        FROM matches m 
        LEFT JOIN users u ON m.player = u.name 
        WHERE DATE_SUB(CURDATE(), INTERVAL 1 MONTH) <= m.match_datetime 
        GROUP BY player 
        ORDER BY turns_taken ASC 
        LIMIT 5
    ";
}

if (!($result = $mysqli->query($query))) {
    showerror($mysqli->errno, $mysqli->error);
} 

$top_players = array();

while ($row = $result->fetch_assoc()) {
    $player_name = $row["player"];
    $turns_taken = $row["turns_taken"];
    $top_players[] = array("name" => $player_name, "turns_taken" => $turns_taken);
}

echo json_encode($top_players);

$mysqli->close();
?>
