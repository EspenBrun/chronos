import React, { Component } from 'react';

export class Import extends Component {
  static displayName = Import.name;

onSubmit = () => {return true}

  render () {
    return (
    <form method="post" encType="multipart/form-data" action="/api/import">
        <div class="form-group">
            <div class="col-md-10">
                <p>Upload one or more files using this form:</p>
                <input type="file" name="files" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-10">
                <input type="submit" value="Upload" />
            </div>
        </div>
    </form>
    );
  }
}