import pandas as pd
import seaborn as sns
import spacy
import re
import pickle
from tqdm import tqdm
from wordcloud import WordCloud
import matplotlib.pyplot as P
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.model_selection import train_test_split
from sklearn.naive_bayes import BernoulliNB
from sklearn.metrics import confusion_matrix, classification_report
import numpy as np
import tensorflow as tf
import keras
from keras.models import Sequential
from keras.models import load_model
from keras import layers
from tensorflow.keras.optimizers import RMSprop, Adam
from tensorflow.keras.preprocessing.text import Tokenizer
from keras import regularizers
from keras import backend as K
from keras.callbacks import ModelCheckpoint
from keras.utils import pad_sequences, to_categorical
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel


app = FastAPI()

# Model for incoming data
class Comment(BaseModel):
    text: str

#load the tokenizer
try:
    with open('tokenizer.pickle', 'rb') as file:
        tokenizer = pickle.load(file)
except FileNotFoundError:
    print("Error: The file tokenizer was not found.")
    tokenizer = None
except Exception as e:
    print(f"An error occurred: {e}")
    tokenizer = None

#load the model
try:
    model = load_model('model30122024.h5')
except FileNotFoundError:
    print("Error: The file model was not found.")
    model = None
except Exception as e:
    print(f"An error occurred: {e}")
    model = None

# Model analysis function
max_words = 5000
max_len = 500

# Model analysis function
def analyze_comment(comment: str) -> str:
    """
    Analyzes the sentiment of a given comment and returns the sentiment as a string.
    Args:
        comment (str): The comment to be analyzed. Must be a non-empty string.
    Returns:
        str: The sentiment of the comment, which can be "Positive", "Negative", or "Neutral".
    Raises:
        ValueError: If the comment is empty or not a string.
    Example:
        >>> analyze_comment("This product is great!")
        'Positive'
    """
    if not comment or not isinstance(comment, str):
        raise ValueError("Invalid comment. Please provide a non-empty string.")
    
    # Tokenize the comment
    comment = tokenizer.texts_to_sequences([comment])
    comment_data = pad_sequences(comment, maxlen=max_len)

    # Make a prediction
    prediction = model.predict(comment_data)[0]  # Get the first element of the prediction array
    predicted_review = np.argmax(prediction)  # Get the index of the maximum value
    print(prediction)
    print(predicted_review)
    # Return the prediction based on the class index
    if predicted_review == 1:
        return "Positive"
    elif predicted_review == 0:
        return "Negative"
    else:
        return "Neutral"

# Endpoint for text analysis
@app.post("/analyze")
async def analyze(comment: Comment):
    try:
        result = analyze_comment(comment.text)
        return {"result": result}
    except Exception as e:
        # Log the error for more insight
        print(f"Error occurred: {e}")
        raise HTTPException(status_code=500, detail=f"Internal Server Error: {e}")

    
# TEST
@app.get("/")
def index():
    return {"details": "Hello, Sasha!"}