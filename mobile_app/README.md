# Word games - Unity mobile application




# Android
## Preparing project
- install latest stable version of Unity with UnityHub, for installation select module **Android Build Support** also the submodules **Android SDK & NDK Tools** and **OpenJDK**
- add project to UnityHub, if installed version si already newer then the project's, confirm for automatic update
- in **File** -> **Build Settings** check if Android platform is selected, should be by default
- on the same screen, there is **Player settings...** then on left menu **Player** click tab ***Other settings***; here you can set package name
- in tab **Publishing settings** create new keystore if you changed package name

## Building project

### Unity
- make sure you downloaded latest **google-services.json** file from  **Firebase** console for set package name
- enable **Google** authorization in **Firebase** console ( *Authorization* -> *Sign-in method* )
- change **Web Client Id** (game object *Canvas*, *Google Signed In* script) on scenes **Intro** and **Settings/MenuNew** to match **client_id** (the one with **client_type 3** and child of **oauth_client**) from **google-services.json** file
- URLs for backend can be HTTP, but it is recommended to use secure connection/HTTPS; URLs can be changed in **GameSettings.cs** in static method *LoadServerURLs* and variable *API_CONNECTION* which sets the default backend, but is overwritten by language specific endpoint
 
# iOS

## Preparing project
-  install latest stable version of Unity with UnityHub on MacOSX, for installation select module **iOS Build Support**, install **XCode** from AppStore
- add project to UnityHub, if installed version is already newer then the project's, confirm for automatic update
- in **File** -> **Build Settings** select iOS platform and confirm change
- on the same screen, there is **Player settings...** then on left menu **Player** click tab ***Other settings***; here you can set bundle indentifier

## Building project

### Unity
- make sure you downloaded latest **GoogleService-Info.plist** file from  **Firebase** console for set bundle indentifier
- enable **GameCenter** authorization in **Firebase** console ( *Authorization* -> *Sign-in method* )
- change **Web Client Id** (game object *Canvas*, *Google Signed In* script) on scenes **Intro** and **Settings/MenuNew** to match **CLIENT_ID** from PLIST file
- delete GoogleSignIn directory from within Unity (unless you like build errors because of unresolved dependencies)
- URLs for backend must be HTTPS, HTTP might work in the Unity simulator, but it wont on iOS device itself; URLs can be changed in **GameSettings.cs** in static method *LoadServerURLs* and variable *API_CONNECTION* which sets the default backend, but is overwritten by language specific endpoint
- if there are errors in Console Log mentioning **com.unity.timeline**, go to project folder -> *Library* -> *PackageCache* and delete folder starting with *com.unity.timeline*
- Build (will make Xcode project and open Finder at its location)
- open file with filename extension *xcworkspace*, Xcode should start

### Xcode
- [setup your Apple developer account](https://help.apple.com/xcode/mac/current/#/devaf282080a?sub=dev8877b4398)
- before building, go *Unity-iPhone* -> *Signing & Capabilities*, check *Automatically manage signing* and  select your *Team*, because Unity will not have the correct team identifier, unless you set it beforehand
- connect iOS device or choose simulator 
- press build/play button :)

