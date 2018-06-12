import React, { Component } from 'react';
import { Row, Col, Button, FormControl, FormGroup, ControlLabel, Alert } from 'react-bootstrap';
import { API_ROOT } from './api-config';

export class Quiz extends Component {
    constructor(props) {
        super(props);
        this.state = { artist: "", title: "", show: false, displayError: false };
        this.handleDismiss = this.handleDismiss.bind(this);
        this.handleShow = this.handleShow.bind(this);
        this.handleTitleChange = this.handleTitleChange.bind(this);
        this.handleArtistChange = this.handleArtistChange.bind(this);
        this.handleFormReset = this.handleFormReset.bind(this);
        this.sendAnswers = this.sendAnswers.bind(this);
    }

    handleTitleChange(e) {
        this.setState({ title: e.target.value });
    }
    handleArtistChange(e) {
        this.setState({ artist: e.target.value });
    }

    handleShow() {
        this.setState({ show: true, displayError: false });
    }

    handleError() {
        this.setState({ show: true, displayError: true })
    }

    handleDismiss() {
        this.setState({ show: false });
    }

    handleFormReset() {
        this.setState({ artist: "", title: "" });
    }

    sendAnswers(e) {
        e.preventDefault();
        this.handleDismiss();
        var url = `${API_ROOT}/Lyrics/`;
        

        fetch(url, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                Artist: this.state.artist,
                Title: this.state.title,
                })
            })
            .then(response => response.json())
            .then((result) => {
               
                if (result.isCorrect) {
                    this.handleShow();
                }
                else {
                    this.handleError();
                }
                this.handleFormReset();
            },
            (error) => {
                this.handleShow();
            })
    }

    render() {
        if (this.state.show) {
            return (

                <Row id="quizForm">
                    <Col sm={12}>
                        <ChooseAlert displayError={this.state.displayError} onDismiss={this.handleDismiss} />
                        <form>
                            <FormGroup>
                                <ControlLabel>Artist</ControlLabel>
                                <FormControl
                                    type="text"
                                    value={this.state.artist}
                                    placeholder="Enter the artist"
                                    onChange={this.handleArtistChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>Song Title</ControlLabel>
                                <FormControl
                                    type="text"
                                    value={this.state.title}
                                    placeholder="Enter the song title"
                                    onChange={this.handleTitleChange}
                                />
                            </FormGroup>
                            <Button bsStyle="success" onClick={this.sendAnswers} > Submit</Button>
                        </form>
                    </Col>
                </Row>
            )
        }
        else {
            return (

                <Row id="quizForm">
                    <Col sm={12}>
                        <form>
                            <FormGroup>
                                <ControlLabel>Artist</ControlLabel>
                                <FormControl
                                    type="text"
                                    value={this.state.artist}
                                    placeholder="Enter the artist"
                                    onChange={this.handleArtistChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>Song Title</ControlLabel>
                                <FormControl
                                    type="text"
                                    value={this.state.title}
                                    placeholder="Enter the song title"
                                    onChange={this.handleTitleChange}
                                />
                            </FormGroup>
                            <Button bsStyle="success" onClick={this.sendAnswers} > Submit</Button>
                        </form>
                    </Col>
                </Row>
            )
        }
    }
}

export class SuccessAlert extends Component {

    render() {
        return (<Alert bsStyle="success" onDismiss={this.props.onDismiss}>Success! You got the answer correctly! You are a musical genius!
        </Alert>);
    }
}

export class ErrorAlert extends Component {

    render() {
        return (<Alert bsStyle="danger" onDismiss={this.props.onDismiss}>Please try again. You are still musically challenged it seems.
                    </Alert>);
    }
}

export class ChooseAlert extends Component {
    constructor(props) {
        super(props)
        
    }

    isError = this.props.displayError;

    render() {
        if (this.isError === null) {
            return null
        }
        else if (!this.isError) {
            return <SuccessAlert onDismiss={this.props.onDismiss} />
        }
        else {
            return <ErrorAlert onDismiss={this.props.onDismiss} />
        }
    }
}