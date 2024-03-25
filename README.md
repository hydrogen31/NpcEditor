# NpcEditor
Türkçe okumak için [buraya](README_TR.md) tıklayınız.

Editing the hitboxes of NPCs in the game is quite a complex task. When you want to adjust one of them, you have to make intense calculations for x, y, width, and height.
I developed this application to ease the editing process. With this application, you can adjust and preview NPC areas flawlessly.

The application allows selecting areas over the character by loading its unique animations and, after selection, placing the new coordinates over the character to display the new hitbox area.

The functions of the application are not limited to hitbox editing. You can also use this tool to learn the action names of characters or watch the movement of these actions.

Usage Video
----------------
[![Usage video](https://img.youtube.com/vi/gXL1wEztdQI/0.jpg)](https://www.youtube.com/watch?v=gXL1wEztdQI)


## Features
- NPCs can be listed from the database, searchable by name or NPC ID.
- Information such as the ID, Script, and hitbox values of the selected NPC can be retrieved.
- The NPC swf is automatically detected and loaded from the resources.
- NPC animations can be listed.
- NPC animations can be played.
- By selecting the multi-selection feature, you can bulk change the hitbox of NPCs with the same ModelID.
- NPC hitbox can be displayed and edited.

## How to Use
### Configuration
Before running the program, you should edit the SQL connection string and the resource address in the `NpcEditor.exe.config` file. The resource address can be an internet address or a local folder on your device. If you have resource files, it is recommended to use them locally for faster loading.

Example resource address:
- Local: C:\Server\Resource (Replace it with your own address)

### NPC Selection
NPCs are loaded from the database and listed at the bottom left. You can search the list by typing **ID** or **name** in the top-left section. A more comprehensive search section may be added in the future.

### NPC Action Movie
The animations of characters vary from one NPC to another. These unique action names of characters are automatically loaded from the living swf. You can play them by selecting from the Action List. As a suggestion, I recommend watching the **cry** animation for more accurate hitbox adjustments. Since characters shrink when hit, this animation played when hit allows you to select a more realistic area.

NOTE: If you are developing a server, you will need these action names in NPC scripts. You can use the names in the PlayMovie functions in your server project.

### NPC Hitbox Selection
When you select a new NPC, existing hitboxes from the database are displayed. You can easily define the hitbox area by clicking on a point on the animation with the mouse and drawing a rectangle. You can also adjust the `x`, `y`, `width`, and `height` values of the hitbox at the bottom right for fine-tuning. To clear your temporary selection and return to the saved state, press the `reset` button. Don't forget to `save` when you finish your selection.

I marked a point at the center of the NPC's animation. This point may sometimes help you. The dimensions of the NPC are calculated based on this point.

If you enable the multi-selection feature, you can select more than one NPC from the list and save the same hitbox values for each of them at once. In this way, you do not have to try to arrange NPCs of different difficulty levels separately. The main condition here is that the characters use the same model. To avoid assigning the same hitbox to a wrong character, you cannot select characters with different ModelIDs from the list.

## How it Works
The application consists of two parts. The main operations are done in a C# project called NpcEditor. I used Flash Player to process Living SWF files. I use Flash.ocx to play them.

NPC animations, rectangle selection, and similar operations are performed in a helper Flash project called NpcLoader.

These two separate projects communicate via flash calls.

## Development
The main application was developed in C#. I designed a helper tool using ActionScript for animations.

You can develop the C# project with **Visual Studio 2019**. The application requires Flash Player. Since the Flash object may cause issues in the latest versions of VS, you may need to avoid using versions beyond VS 2019.
I used **FlashDevelop** to compile the helper swf project called NpcLoader. If you want to develop, you need to replace the existing Flash.ocx with the debugger version to see possible errors and logs.

## Contributors
The entire development process was done by hydrogen31. I took inspiration from Zephyr's [video](https://www.youtube.com/watch?v=W3OLUQuxwG8) while designing the application.
