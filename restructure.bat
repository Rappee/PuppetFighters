:: Structure:
:: 
:: /Assets
::     /Scenes
::     /Scripts
::     /Materials
::     /Images
::     /Characters
::         /Fred
::             /Images
::             /Animations
::             /SpriteAnimations
::
:: Usage: place this .bat file in project folder
@echo off
cd Assets
if not exist "Scenes\*" mkdir Scenes
if not exist "Scripts\*" mkdir Scripts
if not exist "Materials\*" mkdir Materials
if not exist "Images\*" mkdir Images
if not exist "Characters\*" mkdir Characters

cd Characters
if not exist "Fred\*" mkdir Fred
if not exist "Images\*" mkdir Images
if not exist "Animations\*" mkdir Animations
if not exist "SpriteAnimations\*" mkdir SpriteAnimations

PAUSE