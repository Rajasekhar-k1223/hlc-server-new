
import easyocr
import sys
import json
import os

# Initialize reader once to save time on subsequent calls if kept alive
# For this script, it initializes every time it's called
reader = easyocr.Reader(['en'], gpu=False, verbose=False) # Set gpu=True if CUDA is available

def extract_text(file_path):
    try:
        if not os.path.exists(file_path):
            return {"error": "File not found"}
            
        result = reader.readtext(file_path)
        
        # EasyOCR returns a list of tuples: (bbox, text, prob)
        # We just want the text
        extracted_text = " ".join([text for bbox, text, prob in result])
        
        return {"text": extracted_text, "success": True}
    except Exception as e:
        return {"error": str(e), "success": False}

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(json.dumps({"error": "No file path provided"}))
        sys.exit(1)
        
    file_path = sys.argv[1]
    result = extract_text(file_path)
    print(json.dumps(result))
