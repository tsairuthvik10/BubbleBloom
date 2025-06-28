Bubble Bloom - Hackathon Submission
A location-based, procedural AR puzzle game built on the Augg.io platform.

Live Demo Video
(Your 90-second video should be embedded here. It's the most important part of your submission!)

Link to your YouTube/Vimeo Video

Core Concept
Bubble Bloom is an AR game that transforms real-world scanned locations into magical, playable gardens. Players find and pop bubbles to create satisfying chain reactions, scoring points based on skill and strategy. Successful combos are rewarded by growing persistent, beautiful AR plants directly onto the real-world mesh.

Our Design Philosophy
The game is designed around a subtle environmental message: "What you care for, grows." Instead of lectures, this message is delivered through game mechanics. The AR gardens that players grow are a direct result of their skillful play and engagement. The act of playing the game—of engaging with and caring for the space—is what brings beauty and life into the world. This creates a gentle, positive feedback loop that encourages players to feel a sense of ownership and stewardship over their local digital environment.

Game Loop Flow
The core gameplay follows a session-based loop designed for tension and reward. The player's goal is to survive the countdown timer by creating chain reactions to add more time, achieving the highest score possible.

+-------------------------------------+
| 1. Round Begins                     |
|    (Timer starts counting down)     |
+-------------------------------------+
                  |
                  v
+-------------------------------------+
| 2. Player finds and pops a bubble   |
|    to start a combo.                |
+-------------------------------------+
                  |
                  v
+-------------------------------------+
| 3. Chain reaction spreads           |
|    - Score & Combo increase         |
|    - Time is added to the clock     |
+-------------------------------------+
                  |
                  v
+-------------------------------------+
| 4. Combo ends (times out)           |
|    - If combo was > 3, a            |
|      Plant grows as a reward        |
+-------------------------------------+
                  |
                  v
+-------------------------------------+
| 5. Player finds a new bubble        |
|    to start the next combo          |
|    (Return to Step 2)               |
+-------------------------------------+
                  |
                  v
+-------------------------------------+
| 6. Main Timer Reaches Zero          |
|    (Round Over)                     |
+-------------------------------------+
                  |
                  v
+-------------------------------------+
| 7. Final Score is submitted         |
|    to the Leaderboard               |
+-------------------------------------+

How to Run
The project is built with Unity 2022.3.x LTS. The entire game's startup logic is handled by the GameCoordinator.cs script, which has a simple boolean toggle to switch between testing and the final AR build.

Editor Test Scene:

Open the Unity Project.

Navigate to the _Scenes/Game_Development_Scene.

Select the GameCoordinator GameObject in the Hierarchy.

In the Inspector, ensure Is Editor Test Mode is checked ON.

Press Play. You can use your mouse to simulate taps.

Final AR Build:

The final build uses a clean duplicate of the Augg.io Main Scene, named OnSite_AR_Scene.

In this scene, select the GameCoordinator GameObject.

In the Inspector, ensure Is Editor Test Mode is checked OFF.

Set the Expected Anchor Count In AR to match the number of anchors you placed in the Augg.io CMS. This allows the coordinator to know when all bubble clusters have been spawned.

Technical Architecture
Platform: Unity Engine with the Universal Render Pipeline (URP).

AR Framework: Augg.io SDK for real-world LiDAR scanning, cloud anchor management, and on-site localization.

Core Gameplay:

Anchor-Based Spawning: A custom BubbleClusterSpawner generates dynamic bubble puzzles in a radius around each real-world cloud anchor.

Chain-Reaction Engine: A performant system for calculating and visualizing combo chains with animated "energy trails."

Game Coordinator: A robust central controller manages the game state, ensuring the timer only starts after all bubbles have been procedurally spawned.

Backend: Google Firebase (Firestore) for a persistent, scalable global leaderboard system with anonymous user authentication.

Future Work
Real-time Multiplayer: Implementing shared "Golden Bubbles" that all players in an area can race to pop.

AI-Generated Content: Using the Gemini API to generate unique lore and descriptions for each plant the player grows.

Deeper Progression: Adding a Daily Quest system and long-term Achievements to drive player retention.