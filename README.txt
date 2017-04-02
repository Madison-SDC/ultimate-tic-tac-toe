I've made Ultimate Tic-Tac-Toe! Play the unique strategy game with friends on one device! Easily reset the entire board, preview your next move, and undo the most recent move!


Planned features include:
	Instructions! Learn how to play the game with a short, interactive tutorial.
	A menu screen! Make the game feel more complete with a simple menu.
	Sounds and animation! For added immersion.
	Custom pieces! Change colors and shapes to make the game match you.
	Artificial intelligence! This is a bit of a stretch goal, but now you can play the game even if no friends are around.
	Online functionality! A real stretch goal as I've never done anything with networks, but it'd be a great learning experience!
	


Changelog:

UT_0.4.1 (Intuitive patch, unreleased)
	All platforms
		Cleaned up the board image (now symmetrical)
		Animated reset
			Pieces are removed every 0.1 seconds
			Can be stopped by tapping reset again
		Reworked undo functionality
			Clicking undo first undoes "preview" move
			If no preview move, functions as before
		Reworked preview functionality
			No longer highlights the next playable board(s)
			Instead outlines them in the color of the next player
			
	Android
		Increased default quality to "fantastic" from "simple"
		

UT_0.4(Confirm update, 2017-03-28)
	All platforms
		Added confirmation functionality! 
			Clicking a spot now previews the next move by highlighting the next active board
			The clicked spot changes image to reflect the active player (as though the player has moved there)
			The local board of the clicked spot may change color to reflect a new winner (if the previewed move wins the board)
			The global board may change color to reflect the new winner (if the previewed move wins the entire game)
			Click confirm to make your move official (don't worry, this can still be undone with "Undo")
		Rearranged UI for increased useability and readability
		

UT_0.3 (Undo update, 2017-03-25)
	All platforms
		Implemented undo functionality!
		Recolored completed boards a bit more to make difference between claimed & enabled, claimed & disabled, and unclaimed & enabled clearer
		Lots of backend restructuring that doesn't concern the end-user
		Recolored background to black to match board lines and also to look cooler of course


UT_0.2.1 (2017-03-14)
	All platforms
		Changed completed board background colors to make enabled board clearer
		

UT_0.2 (Colors update, 2017-03-14)

	All platforms
		Changed background board image to reflect traditional tic-tac-toe, visualize local/global boards easier
		Empty spaces are now invisible by default
		Highlighting empty spaces shows player color
		Enabled and completed local board now "lighten" to show that they are active
		Global board color resets upon reset
		Changed icon color to red for better contrast

	Android
		Switched to landscape left instead of landscape right
		
	Bugfixes
		Fixed #1 (local games can still be played even when global game is over)
		

UT_0.1.1 (Android exclusive)
	Android
		Minor fixes (honestly not quite sure what)

UT_0.1 (Initial build, 2017-03-11)

	All platforms
		Create game of Ultimate Tic-Tac-Toe with red 'x's and blue 'o's
		Reset upon right click or click of reset button
		Highlight winner of local boards and global board

	Found bugs
		1: Local games can still be played even when global game is over (global winner cannot change)