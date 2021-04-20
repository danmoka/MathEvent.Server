import React from "react";

const Info = ({ title, text }) => {
    return (
        <div className="info">
            <div className="section-title text-center">{title}</div>
            <hr/>
            <div className="main-text">{text}</div>
        </div>
    );
};

export default Info;