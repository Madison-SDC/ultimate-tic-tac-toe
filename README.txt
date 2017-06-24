I've made Ultimate Tic-Tac-Toe! Play the unique strategy game with friends or an AI on one device! Easily reset the entire board, preview your next move, undo the most recent move, and redo any recent moves just in case you made a mistake! Accidentally reset the entire board? Just hit redo a whole bundle of times!


Planned features include:
	Instructions! Learn how to play the game with a short, interactive tutorial.
	Sounds and animation! For added immersion.
	Custom pieces! Change colors and shapes to make the game match you.
	Online functionality! A real stretch goal as I've never done anything with networks, but it'd be a great learning experience!
	


Changelog:

UT_???
	All platforms (for newer versions, "All platforms" will be implied)
		Centered Player panes on Settings scene
		

UT_0.8 (Embrace the overhaul update, 2017-06-21)
	All platforms
		Completely rewrite architecture
			Now built on an MVC model with event support
		Better artificial intelligence
			Monte Carlo Tree Search with variable timing (more time = more difficult)
			Benchmark runs about 650 simulations per second on a blank game
		Settings scene
			Removed unnecessary "Easy/Hard" buttons
			Changed slider handles to circles
			Choose how long to give the AI to think: 0 to 5 seconds
			Change color scheme to black/white to match other scenes
			View now matches previous settings when coming back to this scene
		Menu scene
			Add animated game between two random AIs
		Miscellaneous
			Changed coloring scheme, global game wins are now darker
			
	Known bugs:
		Undo/redo/reset with Monte Carlo AI does not work

UT_0.7 (Gameplay update, 2017-05-06)
	All platforms
		Rule change
			If a local game is over, that board is always disabled
			Sending a player to a game-over board activates all other valid boards
		Added status bar
			Gives info on the last turn (who made it, where it was)
			Gives info on whose turn it is (X or O, AI or human)
		AI improvements
			AI now looks some number of moves into the future (currently 5 by default)
			AI now weighs sending its opponent to a completed board, allowing its opponent to play on any board

UT_0.6.2 (Bugfix patch, 2017-04-26)
	All platforms
		AI now weighs value of winning a local board
		AI now weighs value of blocking opponent from winning local board
	
		Bugfixes
			Fix for game not recoloring when a tie occurred
			Fix for game not allowing undo when game was ended in single player
			Fix for undo being possible after AI previewed its move
			Other misc. bugfixes probably

UT_0.6.1 (Heuristic patch, 2017-04-17)
	All platforms
		Improved artificial intelligence
			I've changed the working name of Hug to Little Hug. Little because it doesn't think ahead, and its mind is little.
			Little Hug now considers its moves based on an heuristic that weighs whether each spot is a local corner, side, or center.

UT_0.6 (AI framework update, 2017-04-10)
	All platforms
		Added Artificial Intelligence player!
			Its working name is "Hug" (as its symbol is an 'O')
			It moves randomly as of right now
			Hug waits a certain amount of time to preview its move
			Hug waits a bit more to confirm its move.
		Added Menu
			Access One Player or Two Player game modes from the new simple menu
			Access the Menu from the Game using the new "Menu" button

UT_0.5 (Intuitive update, 2017-04-02)
	All platforms
		Added redo functionality
			Replay any confirmed move that you undo
			Replay any number of moves that are undone
			Once a new move is played, redo stack is cleared
				Can only redo up to most recent new move
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