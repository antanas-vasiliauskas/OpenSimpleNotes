export type Note = {
  id: string;
  title: string;
  content: string;
  isPinned: boolean;
  createdAt: string; // ISO string format
  updatedAt: string; // ISO string format
};