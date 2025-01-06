from reviewAssesment import analyze_comment, tokenizer, model

# Sample comment for testing
comment = "hate this game"

# Ensure the tokenizer and model are loaded before analyzing
if tokenizer and model:
    try:
        # Analyze the comment using the existing analyze_comment function
        result = analyze_comment(comment)
        print(f"Prediction for comment: {result}")
    except Exception as e:
        print(f"Error analyzing comment: {e}")
else:
    print("Model or Tokenizer not loaded properly.")
