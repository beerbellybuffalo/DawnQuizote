# Dawn Quizote: Submission for Epyphite Corp Take Home Assignment

## Assignment Brief / Problem Statement
The demand for engaging and interactive learning tools is increasing, especially on mobile platforms where accessibility and usability are critical. Traditional quiz applications often lack immersive and engaging elements, making it challenging to maintain user interest and motivation. Leveraging Unity 3D to create a visually appealing and gamified quiz application for mobile can address this need.

To enhance the experience further, integrating Augmented Reality (AR) can provide a novel and engaging way for users to interact with quiz content in their environment, promoting a deeper connection with the material. However, AR is optional, as a well-designed standard mobile quiz app with interactive features, animations, and gamified elements can still achieve the goal of improving user engagement and retention.

### The key challenges include:
> Designing an intuitive, visually appealing user interface for mobile devices.
> Ensuring smooth performance across various mobile platforms (iOS and Android).
> Providing a flexible system to add, update, and manage quiz questions and categories.
> Optionally, implementing AR features to enhance immersion without overwhelming development resources.
> Ensuring the app is scalable, lightweight, and user-friendly for a broad audience.
> The solution should balance functionality, interactivity, and performance, aiming to create a quiz application that is engaging, accessible, and enjoyable to use.
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
## About my Submission
Dawn Quizote is a parkour enthusiast and Pop Quiz aficionado. Help her get good at both in this gamified quiz app that combines subway surfer with a traditional quiz app like QuizUp or Kahoot!
This is a link to the [figma prototype](https://tinyurl.com/danielEpyphiteAppMockup)

You can find the final builds for Android and iOS under Releases.
Android builds can be installed and run directly onto Android devices. Unfortunately, because I don't have an active Apple Developer account, I cannot upload the app directly to the App Store, or provide a test link via Test Flight to install iOS builds. I can only provide the compiled program files in a zipped folder under the Releases section of this repository.

## Design Justification
![Dawn_Running](https://github.com/user-attachments/assets/3c5ea703-5e27-45f0-8f80-7f88e5e95087)  
[![Demo Video](https://img.youtube.com/vi/hFnKn5Q473E/0.jpg)](https://youtube.com/shorts/hFnKn5Q473E)    
**This is a preview of the gameplay, the above image links to a youtube video.**

["Subway Surfers"](https://poki.com/en/g/subway-surfers) is a mobile game that falls under the genre of Endless Runner, where the goal is to keep running for as long as possible while collecting coins and avoiding hazards. My quiz app follows this running format, but with a key difference that each round of gameplay ends when all questions in a quiz have been attempted.
The reason for this format is to **use the 3D aspect of Unity in a way that increases user engagement, even though 3D is not strictly required for a quiz app**. The dynamic movement in the background and the idea of doing parkour while answering questions adds to a sense of urgency which is helpful for quizzes that are timed.

## Quiz Gameplay
The Quiz Selection Page offers two categories of quizzes: Popular Quizzes or My Quizzes. Popular Quizzes are quizzes that are available in the app by default, whereas My Quizzes refers to player-generated content.
When the player selects a quiz, they will begin a parkour sequence which will continue until they have attempted all questions for that quiz.

The correct answer to each question must be provided within a specified time limit (10s per question by default), for the runner to clear the obstacle parkour-style. Each correct answer will add to a total score, with quicker answers being rewarded with more points. Otherwise, if an incorrect answer is given or they have run out of time, they will stumble/collide with the obstacle and not receive any points for the question. At the end, the player will be shown their final score.

```diff
! **You may try out the Mental Sums quiz under Popular Quizzes to see the gameplay, or add your own quiz by clicking on Create New Quiz**
```

## Features as of v1.0.5
✔️ = Completed  
⌛ = In Progress  
### Quiz-related
(*required in brief) 1. Flexible system to add, update, and manage quiz questions and categories  
✔️Browse Existing Quizzes  
✔️Create New Quiz  
✔️Delete All Quizzes  
✔️Delete Single Quiz  
⌛Edit Single Quiz  
### General
✔️Quiz Main Gameplay  
✔️Pause App  
✔️Quit App  
✔️High Score Indicator per quiz so the player can track if they have improved  
✔️Menu Settings to adjust music and sfx  
✔️Parkour Animations  
✔️Countdown Timer UI  
