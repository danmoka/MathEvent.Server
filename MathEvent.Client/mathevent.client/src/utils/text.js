const cropText = (length, text) => {
  if (text.length > length) {
    const croppedText = `${text.substring(0, length)}...`;

    return {
      text,
      croppedText,
    };
  }

  return {
    text,
    croppedText: null,
  };
};

const splitText = (length, text) => {
  if (text) {
    const pieces = text.split(' ');
    const newPieces = [];

    pieces.forEach((piece) => {
      if (piece.length > length) {
        const splittedPieces = piece.match(new RegExp(`.{1,${length}}`, 'g'));

        newPieces.push(...splittedPieces);
      } else {
        newPieces.push(piece);
      }
    });

    return newPieces.join(' ');
  }

  return text;
};

export { cropText, splitText };
