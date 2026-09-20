import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

export interface TicketDeleteDialogData {
  title: string;
}

@Component({
  selector: 'app-ticket-delete-dialog',
  imports: [MatButtonModule, MatDialogModule],
  template: `
    <h2 mat-dialog-title>Delete ticket?</h2>

    <mat-dialog-content>
      <p>
        The ticket <strong>{{ data.title }}</strong> and its persisted AI analysis will be
        permanently deleted.
      </p>

      <p>This operation cannot be undone.</p>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button type="button" [mat-dialog-close]="false">Cancel</button>

      <button mat-flat-button type="button" [mat-dialog-close]="true">Delete ticket</button>
    </mat-dialog-actions>
  `,
})
export class TicketDeleteDialog {
  protected readonly data = inject<TicketDeleteDialogData>(MAT_DIALOG_DATA);
}
