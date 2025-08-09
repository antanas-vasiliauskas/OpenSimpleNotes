import React, { useEffect } from 'react';

/**
 * Renders the official GitHub "Star" button using the script from buttons.github.io
 * Loads the script only once per page.
 */
const GitHubButton: React.FC = () => {
  useEffect(() => {
    if (!document.getElementById('github-buttons-script')) {
      const script = document.createElement('script');
      script.id = 'github-buttons-script';
      script.async = true;
      script.defer = true;
      script.src = 'https://buttons.github.io/buttons.js';
      document.body.appendChild(script);
    }
  }, []);

  return (
    <a
      className="github-button"
      href="https://github.com/antanas-vasiliauskas/OpenSimpleNotes"
      data-icon="octicon-star"
      data-show-count="true"
      aria-label="Star antanas-vasiliauskas/OpenSimpleNotes on GitHub"
    >
      Star
    </a>
  );
};

export default GitHubButton;
