
# NBD Project

Application that automatically assesses whether a written game review is positive or negative (similarly, to what review system on platforms such as Steam looks like). 
For simplicity of the project only reviews in English and written using characters from Latin alphabet are taken into account. In our project we use a simple LSTM.

This assessment/prediction is done by a trained AI model after the user gives input (in form of text in the text field).

The application is a demo including a sign-up/login, the AI-review tool and an access to history of reviews. All of the database parts (for example storing history and login info)
are managed by a non-relational database.

This project was done for NBD subject at PJAIT in Warsaw.

Authors: 
O.Podolich - s22590;
J.Malicki - s22264


## Table of contents
* [Tech Stack](#tech_stack)
* [Setup](#setup)
* [File Description](#File_Description)  
* [Sources](#sources)

## Tech_Stack

**Client:** Unity 2022.3.46f1

---
**Database:** Firebase DB for Unity

https://github.com/firebase/firebase-unity-sdk

FirebaseDatabase & FirebaseAuthentication

https://firebase.google.com/docs/database/unity/start

https://firebase.google.com/docs/auth/


---
**Backend:** Uvicorn, FastAPI

 https://www.uvicorn.org/

---
**AI Model:** LSTM - Long Short-Term Memory
## Setup

Make sure that you have Docker installed on your machine.

If it is not, you can downlolad it for example here:
https://www.docker.com/products/docker-desktop/

---
To run the file, go into the directory:
"NBD_PROJECT/aiModel"

In this directory run the following command:

"docker compose up --build     - Build docker"

or

in the same directory run:

"uvicorn reviewAssesment:app --reload --port=8000 --host=0.0.0.0"

This command should create a docker container named "aimodel" and run it. Also, it will run the requirements.txt file and install all the needed packages. Afterwards, it will run the background app for processing input with AI model.

---
Run the reviewAssesment.py through uvicorn or Docker as explained above.
Next, run the app build under the name "NBD_Project/Build/Unity_NBD_Project.exe".

## File_Description

### aiModel
- testModel.py

This file is a script that loads a pre-trained machine learning model and a tokenizer, and uses them to analyze the sentiment of a given comment. Here's a breakdown of what it does:

Loading the Tokenizer: It attempts to load a tokenizer from a pickle file (tokenizer.pickle). The tokenizer is used to convert text into sequences of integers.
Loading the Model: It attempts to load a pre-trained Keras model from an HDF5 file (model30122024.h5). This model is used to predict the sentiment of the input text.

Model Analysis Function:

Function Definition: analyze_comment(comment: str) -> str is defined to take a string input (comment) and return a string indicating the sentiment.
Input Validation: It checks if the input is a non-empty string.
Tokenization: It converts the input comment into a sequence of integers using the loaded tokenizer.
Padding: It pads the tokenized sequence to ensure it has a fixed length (max_len).
Prediction: It uses the loaded model to predict the sentiment of the padded sequence.
Result Interpretation: It interprets the model's prediction and returns "Positive", "Negative", or "Neutral" based on the predicted class.
Testing the Function: The script calls analyze_comment with a sample comment to test the function and print the prediction.

- MODEL_Generator.ipynb
  
The MODEL_Generator.ipynb file is a Jupyter Notebook that installs necessary Python packages, processes data, and generates a machine learning model for sentiment analysis. Here's a brief overview of its functionality:

Package Installation:

Installs required packages such as spacy, wordcloud, keras, seaborn, tensorflow, matplotlib, pandas, and scikit-learn using %pip install.

Imports various libraries for data manipulation (pandas), visualization (seaborn, matplotlib), natural language processing (spacy, re), progress tracking (tqdm), and machine learning (sklearn, tensorflow, keras).

Loads and processes text data using pandas.
Tokenizes and vectorizes text data using spacy and sklearn's TfidfVectorizer.
Splits the data into training and testing sets using train_test_split.

Defines a neural network model using keras's Sequential and layers.
Compiles the model with optimizers like RMSprop and Adam.
Trains the model on the processed data.

Evaluates the model's performance using metrics like confusion_matrix and classification_report from sklearn.

Saves the trained model and tokenizer for future use.
This notebook automates the process of setting up the environment, processing data, training a machine learning model, and saving the model for deployment.

- reviewAssesment.py
  
The reviewAssesment.py utilises FastAPI application designed to analyze text comments and determine their sentiment. The sentiment analysis is performed using a pre-trained machine learning model.

The analyze_comment function processes a given comment to predict its sentiment. It first determines the index of the maximum value in the prediction array using np.argmax(prediction), which indicates the most likely sentiment class. The function then prints the prediction array and the predicted class index for debugging purposes. Based on the predicted class index, it returns a string indicating whether the sentiment is "Positive", "Negative", or "Neutral".

The FastAPI application defines an endpoint /analyze that accepts POST requests. This endpoint is handled by the analyze function, which expects a Comment object containing the text to be analyzed. The function calls analyze_comment with the provided text and returns the result in a JSON format. If an error occurs during the analysis, it logs the error and raises an HTTP 500 Internal Server Error with a detailed message.

Additionally, the application includes a test endpoint / that handles GET requests. This endpoint is managed by the index function, which returns a simple JSON response with a greeting message, "Hello, Sasha!". This endpoint can be used to verify that the FastAPI application is running correctly.


- tokenizer.pickle
  
The tokenizer converts the input comment into a sequence of integers that can be later analysed by the model.

- model30122024
  
model30122024 is saved in various formats including (.h5, .keras and .sav). This is the AI model used for processing and analysing the reviews.
This model is saved and loaded in various files.

### pythonDataAnalysis

- data_dashboard.ipynb

This file is a Jupyter Notebook designed for showcasing and analyzing text data, specifically for sentiment analysis. 
It starts by importing necessary libraries and reading a CSV file containing the training data. The notebook then explores the data, showing samples and label distributions with visualizations. It cleans the text data by removing stopwords and lemmatizing words. Word clouds are generated to visualize common words in positive and negative reviews. Finally, it loads a pre-trained model, tokenizes the text data, and makes predictions to evaluate the model's performance.

Some examples of word analysis you can find below.

![image](https://github.com/user-attachments/assets/2915c905-1f1e-4505-bdf1-e54cad439fba)

![image](https://github.com/user-attachments/assets/211bd447-36d5-4ff9-a381-a4ecf871320c)

![image](https://github.com/user-attachments/assets/7cff4a6d-680b-43f2-9deb-e38b6588977b)

## Process_Model


![canvas_project-basic-sketch-250105_1604](https://github.com/user-attachments/assets/bda6ad28-2aac-4d30-9e5d-16b23e6c55c9)

## Sources

Data for AI-model training was acquired:
https://www.kaggle.com/datasets/andrewmvd/steam-reviews/code 

This project is a continuation of:

[https://github.com/HerrBearlin/PAD_SteamReview?tab=readme-ov-file ](https://github.com/AlexPodolich/PAD_SteamReview)
