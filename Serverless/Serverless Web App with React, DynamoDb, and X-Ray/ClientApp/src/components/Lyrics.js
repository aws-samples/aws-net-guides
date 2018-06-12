import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import { API_ROOT } from './api-config';

export class Lyrics extends Component {
    constructor() {
        super();
        this.state = { lyrics: "" };
        this.getLyrics = this.getLyrics.bind(this);
    }

    getLyrics() {
        fetch(`${API_ROOT}/Lyrics`, {
            headers: new Headers({
                "Accept": "application/json"
            })
        })
            .then(response => response.json())
            .then(lyrics => this.setState({ lyrics: lyrics }))
            .catch(error => console.log(error))
    }

    componentDidMount() {
        this.getLyrics();
    }

    render() {
        let lyrics = this.state.lyrics;

        return (
             
                    <div className="row">
                        <div className="col-md-10">
                            <b>{lyrics}</b>
                        </div>
                        <div className="col-md-2">
                            <Button onClick={this.getLyrics} bsStyle="success">Refresh for new Lyric </Button>
                        </div>
                    </div>
               
            );
    }
}

